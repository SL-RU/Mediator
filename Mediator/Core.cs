using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Mediator
{
    public enum DbType
    {
        Scripts,
        Texts,
        Characters,
        Projects,
        Quest
    }


    public interface IDbInterface
    {
        void Connect();
        void Close();

        /// <summary>
        /// Get element's data
        /// </summary>
        /// <param name="dbName">DB's name</param>
        /// <param name="id">ID</param>
        /// <param name="preview">Get only preview data</param>
        /// <param name="version">Get specific version of data</param>
        /// <returns>Serialized data</returns>
        byte[] Get(string dbName, string id, bool preview = false, string version = null);

        /// <summary>
        /// Set element's data
        /// </summary>
        /// <param name="dbName">DB's name</param>
        /// <param name="id">ID</param>
        /// <param name="data">Serialized data</param>
        void Set(string dbName, string id, byte[] data);
    }

    public class Core
    {
        public IDbInterface Db { get; private set; }

        //public Dictionary<DbType, IData> 

        public IDictionary<DbType, Db<IData>> Dbs { get; private set; }
        

        public Core(IDbInterface db)
        {
            
            Db = db;

            Dbs = new Dictionary<DbType, Db<IData>>();
            //TODO: create normal init
            //foreach (var d in Enum.GetValues(typeof(DbType)))
            //{
            //    Dbs.Add((DbType)d, new Db<>());
            //}
            IReadOnlyCollection<Db<IData>> l = new List<Db<IData>>();
            //l.Add(new Db<CharactersData>(this, "ff", DbType.Characters));
            //TODO: implement db initing

        }
    }

    public interface IDbEditor<out T> where T: IData
    {
        T CurrentlyEditingData { get; }

        void CloseEdit();
    }

    public interface IDbViewer<out T> where T : IData
    {
        void Clear();
    }

    public class Db<T> where T : IData
    {
        public Core Core;
        private Dictionary<string, T> Cache;
        public string DbName { get; protected set; }
        public DbType DbType { get; protected set; }

        public IDbViewer<T> Viewer { get; protected set; }
        public IDbEditor<T> Editor { get; protected set; }

        //TODO: Add null check in Db<T>

        public Db(Core core, string dbName, DbType type)
        {
            Core = core;
            DbName = dbName;
            DbType = type;

            Cache = new Dictionary<string, T>();
        }

        #region Connect
        /// <summary>
        /// Connect visual editor to Db
        /// </summary>
        /// <param name="viewer"></param>
        public void ConnectEditor(IDbEditor<T> editor)
        {
            Editor = editor;
        }

        /// <summary>
        /// Connect visual viewer to Db
        /// </summary>
        /// <param name="viewer"></param>
        public void ConnectViewer(IDbViewer<T> viewer)
        {
            Viewer = viewer;
        }
        #endregion

        #region Serialization
        public byte[] GetSerialized(string id)
        {
            return Core.Db.Get(DbName, id);
        }

        public T GetDeserialized(string id)
        {
            using (var m = new MemoryStream(GetSerialized(id)))
            {
                return (T)Serializer.Deserialize(typeof(T), m);
            }
        }

        public byte[] Serialize(T data)
        {
            using (var m = new MemoryStream())
            {
                Serializer.Serialize(m, data);
                return m.ToArray();
            }
        }

        public byte[] Serialize(string id)
        {
            return Serialize(Get(id));
        }
        #endregion

        #region GET
        //TODO: implement GET
        /// <summary>
        /// Get only preview data. Full data may be presenting in class.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetPreview(string id)
        {
            return default(T);
        }
        /// <summary>
        /// Get full data class by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(string id)
        {
            return default(T);
        }
        #endregion

        /// <summary>
        /// Apply changes of data class to DB
        /// </summary>
        /// <param name="dat"></param>
        public void Set(T dat)
        {
            //dat.Update(null);
            Core.Db.Set(DbName, dat.Id, Serialize(dat));
        }

        public static T Create()
        {
            return default(T);
        }

        /// <summary>
        /// Update cached Data's fields using values of different "data" object
        /// </summary>
        /// <param name="data">"data" object</param>
        private void Update(T data)
        {
            if(data.GetType() != typeof(T))
                throw new Exception("Different types - couldn't update");
            if(data == null)
                throw new NullReferenceException();
            var d = Get(data.Id);
            if (d == null) // It isn't required - only just for the KOSTILY case 
                throw new NullReferenceException();

            if (data.Equals(d))
                return;
            foreach (PropertyInfo oPropertyInfo in d.GetType().GetProperties())
            {
                //Check the method is not static
                if (!oPropertyInfo.GetGetMethod().IsStatic)
                {
                    //Check this property can write
                    if (d.GetType().GetProperty(oPropertyInfo.Name).CanWrite)
                    {
                        //Check the supplied property can read
                        if (oPropertyInfo.CanRead)
                        {
                            //Update the properties on this object
                            d.GetType().GetProperty(oPropertyInfo.Name).SetValue(d, oPropertyInfo.GetValue(data, null), null);
                        }
                    }
                }
            }

        }
    }

}
