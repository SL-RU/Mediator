﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Microsoft.SqlServer.Server;
using ProtoBuf;

namespace Mediator.SQLiteDB
{

    [ProtoContract]
    public class SqliteDbInterfaceVersionData : Extensible
    {
        [ProtoMember(1)]
        public string Locked { get; set; }

        [ProtoMember(2)]
        public string Version { get; set; }

        [ProtoMember(3)]
        public string EditBy { get; set; }

        [ProtoMember(4)]
        public string EditTime { get; set; }
    }

    public class SqliteDbInterface : IDbInterface
    {
        public string CPath { get; private set; }

        public bool IsLocalDb => true;

        public string ProjectFileName => Path.Combine(CPath, "mediator.prj");
        public string DbFileName => Path.Combine(CPath, "current.db");
        public string OldDbDirName => Path.Combine(CPath, "old");
        public string TamplatesDirName => Path.Combine(CPath, "tamplates");

        public string CurrentVersion => "cur";

        public string CurrentAuthor => "me";

        private SQLiteFactory factory;
        private SQLiteConnection connection;


        public SqliteDbInterface(string path)
        {
            CPath = path;
        }

        public async Task Connect()
        {
            if (connection != null)
                return;
            if (!Directory.Exists(CPath))
                throw new DirectoryNotFoundException("Couldn't open DB - path does'n exist");
            if (!File.Exists(Path.Combine(CPath, ProjectFileName)))
                throw new DirectoryNotFoundException("Couldn't create DB - project doesn't exist");

            factory = (SQLiteFactory) DbProviderFactories.GetFactory("System.Data.SQLite");
            connection = (SQLiteConnection) factory.CreateConnection();
            connection.ConnectionString = "Data Source = " + DbFileName;
            connection.Open();
        }

        public void Create()
        {
            if (!Directory.Exists(CPath))
                throw new DirectoryNotFoundException("Couldn't create DB - path does'n exists");
            if (File.Exists(Path.Combine(CPath, ProjectFileName)))
                throw new DirectoryNotFoundException("Couldn't create DB - project already created");

            //using (var f = File.Open(Path.Combine(CPath, ProjectFileName), FileMode.CreateNew, FileAccess.ReadWrite))
            //{
            File.WriteAllText(Path.Combine(CPath, ProjectFileName), CPath);
            //}

            SQLiteConnection.CreateFile(DbFileName);

            factory = (SQLiteFactory) DbProviderFactories.GetFactory("System.Data.SQLite");
            connection = (SQLiteConnection) factory.CreateConnection();

            connection.ConnectionString = "Data Source = " + DbFileName;
            connection.Open();

            var dbs = Enum.GetNames(typeof (DbType));
            foreach (var db in dbs)
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"CREATE TABLE [" + db + @"] (
                        [id] TEXT PRIMARY KEY UNIQUE NOT NULL,
                        [data] BLOB
                        );";
                    command.Prepare();
                    //command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

        }

        public async Task Close()
        {
            connection.Close();
        }

        public async Task<byte[]> Get(string dbName, string id, bool preview = false, string version = null)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                //TODO: Versioning
                command.CommandText = @"SELECT data FROM " + dbName + " WHERE id='" + id + "';";
                command.Prepare();
                command.Parameters.AddWithValue("@db", dbName);
                command.Parameters.AddWithValue("@id", id);
                command.CommandType = CommandType.Text;
                SQLiteDataReader reader = command.ExecuteReader();
                byte[] b = null;
                foreach (DbDataRecord r in reader)
                {
                    //string s = (string)r["data"];
                    b = (byte[])r["data"];
                    break;
                }
                return b;
            }
        }

        public async Task Set(string dbName, string id, byte[] data)
        {
            SqliteDbInterfaceVersionData d;
            using (var m = new MemoryStream(data))
            {
                d = (SqliteDbInterfaceVersionData) Serializer.Deserialize(typeof (SqliteDbInterfaceVersionData), m);
                d.EditBy = CurrentAuthor;
                d.Version = CurrentVersion;
                d.EditTime = DateTime.Now.ToString();
                if (d.Locked == null)
                    d.Locked = "";
            }
            using (var m = new MemoryStream())
            {
                Serializer.Serialize(m, d);
                data = m.ToArray();
            }
            
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT count(*) FROM " + dbName + " WHERE id=@id";
                command.Prepare();
                command.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    command.CommandText = "INSERT INTO '" + dbName + "'('id','data') VALUES (@id, @data);";
                    command.Prepare();
                    command.Parameters.AddWithValue("@db", dbName);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@data", data);
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = "UPDATE '" + dbName + "' SET data=@data WHERE id=@id;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@db", dbName);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@data", data);
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<string> GetNewId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<long> GetCount(string dbName)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT count(*) FROM " + dbName + ";";
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public async Task<List<string>> GetAllIds(string dbName)
        {
            List<string> ids = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT id FROM " + dbName + ";";
                SQLiteDataReader reader = command.ExecuteReader();
                foreach (DbDataRecord r in reader)
                {
                    ids.Add((string) r["id"]);
                }
            }
            return ids;

        }
    }
}