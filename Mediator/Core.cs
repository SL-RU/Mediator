using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace Mediator
{
    /// <summary>
    /// Table type in the DB. every named constant is also a name of DB's 
    /// </summary>
    public enum DbType
    {
        Scripts,
        Texts,
        Characters,
        Dialogs,
        Quest
    }

    public interface IDbInterface
    {
        bool IsLocalDb { get; }

        string CurrentVersion { get; }
        string CurrentAuthor { get; }

        Task Connect();
        Task Close();

        /// <summary>
        /// Get element's data
        /// </summary>
        /// <param name="dbName">DB's name</param>
        /// <param name="id">ID</param>
        /// <param name="preview">Get only preview data</param>
        /// <param name="version">Get specific version of data</param>
        /// <returns>Serialized data</returns>
        Task<byte[]> Get(string dbName, string id, bool preview = false, string version = null);

        /// <summary>
        /// Set element's data
        /// </summary>
        /// <param name="dbName">DB's name</param>
        /// <param name="id">ID</param>
        /// <param name="data">Serialized data</param>
        Task Set(string dbName, string id, byte[] data);

        /// <summary>
        /// Recieve ID for a new element 
        /// </summary>
        /// <returns></returns>
        Task<string> GetNewId();

        Task<long> GetCount(string dbName);
        Task<List<string>> GetAllIds(string dbName);
    }

    public class Core
    {
        public IDbInterface Db { get; private set; }

        //public Dictionary<DbType, IData> 

        private Dictionary<DbType, IDb<IData>> Dbs;

        public Core(IDbInterface db)
        {

            Db = db;

            Dbs = new Dictionary<DbType, IDb<IData>>();

            //TODO: ???
            Dbs.Add(DbType.Characters, new Db<CharacterData>(this, DbType.Characters));
            Dbs.Add(DbType.Scripts, new Db<ScriptData>(this, DbType.Scripts));
            Dbs.Add(DbType.Dialogs, new Db<DialogData>(this, DbType.Dialogs));
            Dbs.Add(DbType.Quest, new Db<CharacterData>(this, DbType.Quest));
            Dbs.Add(DbType.Texts, new Db<TextData>(this, DbType.Texts));
        }

        public IDb<IData> getDb(DbType type)
        {
            return (IDb<IData>)Dbs[type];
        }

        public async void GetCache()
        {
            var t = await Db.GetAllIds(DbType.Texts.ToString());
            foreach (var s in t)
            {
                await ((Db<TextData>) getDb(DbType.Texts)).Get(s);
            }
        }
    }

    public interface IDbEditor<in T> where T : IData
    {
        DbType DbType { get; }
        string CurrentlyEditingDataId { get; }
        void Edit(T edit);
        void CloseEdit();
    }

    public interface IDbViewer<out T> where T : IData
    {
        DbType DbType { get; }
        /// <summary>
        /// Creates(if not created yet) IDbEditor
        /// </summary>
        /// <returns></returns>
        UIElement GetEditor(); //DbViewer is more important. It's controlling all Data and let it create Editor.

        void Clear();
    }

    public interface IDb<out T> where T : IData
    {
        string DbName { get; }
        DbType DbType { get; }
    }

    public class Db<T> : IDb<T>
        where T : Data, new()
    {
        public Core Core;
        private Dictionary<string, T> CacheDict;
        public ObservableCollection<T> Cache;

        public string DbName { get; protected set; }
        public DbType DbType { get; protected set; }

        protected IDbViewer<T> Viewer { get; set; }
        protected IDbEditor<T> Editor { get; set; }

        //TODO: Add null check in Db<T>

        public Db(Core core, DbType type)
        {
            Core = core;
            DbName = type.ToString();
            DbType = type;

            CacheDict = new Dictionary<string, T>();
            Cache = new ObservableCollection<T>();

        }

        public T CacheGet(string id)
        {
            if (CacheDict.ContainsKey(id))
                return CacheDict[id];
            return null;
        }

        private void CacheAdd(T d)
        {
            CacheDict.Add(d.Id, d);
            Cache.Add(d);
        }

        private void CacheRemove(string id)
        {
            if (CacheDict.ContainsKey(id))
            {
                var i = CacheDict[id];
                Cache.Remove(i);
            }
        }
        #region GUI
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
        /// <summary>
        /// Open data in editor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Edit(string id)
        {
            T d = await Get(id);
            await Lock(d); //Lock data
            if(Editor != default(IDbEditor<T>))
                Editor.Edit(d);
        }
        #endregion

        #region Serialization


        public T Deserialize(byte[] s)
        {
            using (var m = new MemoryStream(s))
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

        public async Task<T> ReceiveData(string id, bool preview)
        {
            var b = await Core.Db.Get(DbName, id, preview);
            T d = default(T);
            if (b != null)
                d = Deserialize(b);
            d.Id = id;
            return d;
        }
        #endregion

        #region GET
        //TODO: implement GET
        /// <summary>
        /// Get only preview data. Full data may be presenting in class.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetPreview(string id)
        {
            if (CacheDict.ContainsKey(id))
                return CacheDict[id];

            //if doesn't in cache
            var r = await ReceiveData(id, true);
            if (r != null)
                CacheAdd(r);

            return r;
        }
        /// <summary>
        /// Get full data class from cache or DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Get(string id)
        {
            if (CacheDict.ContainsKey(id))
            {
                if (CacheDict[id].IsFullDataRecieved) //if already full data already recieved
                    return CacheDict[id];
                else
                {
                    var r = await ReceiveData(id, false);
                    Update(r);
                    return r;
                }
            }
            //if doesn't in cache
            var re = await ReceiveData(id, false);
            if (re != null)
                CacheAdd(re);

            return re;
        }
        #endregion

        /// <summary>
        /// Apply changes of data class to DB
        /// </summary>
        /// <param name="dat"></param>
        public async Task Set(T dat)
        {
            //dat.Update(null);
            await Core.Db.Set(DbName, dat.Id, Serialize(dat));
            if (Cache.Contains(dat))
            {
                T d = await ReceiveData(dat.Id, false);
                Update(d);
            }
        }

        /// <summary>
        /// Create new data and get it.
        /// </summary>
        /// <returns>new data</returns>
        public async Task<T> Create()
        {
            if (Core.Db.IsLocalDb) //ONLY FOR LOCAL DB!!! KOSTYL'
            {
                T t = new T(); //Create new data
                t.Id = await Core.Db.GetNewId(); //recieve new ID
                await Set(t); //Register new data on server
                t = await Get(t.Id); //recieve that data with version and etc info
                return t;
            }
            return default(T);
        }

        /// <summary>
        /// Update cached Data's fields using values of different "data" object
        /// </summary>
        /// <param name="data">"data" object</param>
        public void Update(T data)
        {
            if (data.GetType() != typeof(T))
                throw new Exception("Different types - couldn't update");
            if (data == null)
                throw new NullReferenceException();
            var d = CacheGet(data.Id);
            if (d == null) // It isn't required - only just for the KOSTILY case 
                throw new NullReferenceException();

            if (data.Equals(d))
                return;
            foreach (PropertyInfo oPropertyInfo in data.GetType().GetProperties())
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

        /// <summary>
        /// Lock data before edit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task Lock(T data)
        {
            if (Core.Db.IsLocalDb)
                data.Locked = Core.Db.CurrentAuthor;
        }

        /// <summary>
        /// Unock data after edit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task UnLock(T data)
        {
            if (Core.Db.IsLocalDb)
                data.Locked = "";
        }

        /// <summary>
        /// Check before edit
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanEdit(T data)
        {
            if (Core.Db.IsLocalDb)
                return data.Locked == Core.Db.CurrentAuthor;
            else
            {
                return false;
            }
        }

    }

}
