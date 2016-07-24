using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Mediator.SQLiteDB
{
    public class SqliteDbInterface : IDbInterface
    {
        public string CPath { get; private set; }

        public string ProjectFileName => Path.Combine(CPath, "mediator.prj");
        public string DbFileName => Path.Combine(CPath, "current.db");
        public string OldDbDirName => Path.Combine(CPath, "old");
        public string TamplatesDirName => Path.Combine(CPath, "tamplates");

        private SQLiteFactory factory;
        private SQLiteConnection connection;


        public SqliteDbInterface(string path)
        {
            CPath = path;
        }

        public void Connect()
        {
            if (!Directory.Exists(CPath))
                throw new DirectoryNotFoundException("Couldn't open DB - path does'n exist");
            if (File.Exists(Path.Combine(CPath, ProjectFileName)))
                throw new DirectoryNotFoundException("Couldn't create DB - project doesn't exist");

            factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
            connection.ConnectionString = "Data Source = " + DbFileName;
            connection.Open();
        }

        public void Create()
        {
            if (!Directory.Exists(CPath))
                throw new DirectoryNotFoundException("Couldn't create DB - path does'n exists");
            if (File.Exists(Path.Combine(CPath, ProjectFileName)))
                throw new DirectoryNotFoundException("Couldn't create DB - project already created");

            using (var f = File.Open(Path.Combine(CPath, ProjectFileName), FileMode.CreateNew, FileAccess.ReadWrite))
            {
                File.WriteAllText(Path.Combine(CPath, ProjectFileName), CPath);
            }

            SQLiteConnection.CreateFile(DbFileName);

            factory = (SQLiteFactory) DbProviderFactories.GetFactory("System.Data.SQLite");
            connection = (SQLiteConnection) factory.CreateConnection();

            connection.ConnectionString = "Data Source = " + DbFileName;
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                var dbs = Enum.GetNames(typeof (DbType));
                foreach (var db in dbs)
                {
                    command.CommandText = @"CREATE TABLE [" + db + @"] (
                        [id] integer PRIMARY KEY UNIQUE NOT NULL,
                        [data] BLOB
                        );";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }

        }

        public void Close()
        {
            connection.Close();
        }

        public byte[] Get(string dbName, string id, bool preview = false, string version = null)
        {

        }

        public void Set(string dbName, string id, byte[] data)
        {

        }
    }
}