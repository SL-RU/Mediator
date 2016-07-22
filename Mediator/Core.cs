using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
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


    public interface IData
    {
        string Id { get; }

        //void Update(IData data);
    }

    public interface IDbInterface
    {
        /// <summary>
        /// Get element's data
        /// </summary>
        /// <param name="dbName">DB's name</param>
        /// <param name="id">ID</param>
        /// <returns>Serialized data</returns>
        byte[] Get(string dbName, string id);

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

        public Core(IDbInterface db)
        {
            Db = db;
        }
    }

    public class Db<T> where T : IData
    {
        public Core Core;
        private Dictionary<string, T> Cache;
        public string DbName { get; protected set; }
        public DbType DbType { get; protected set; }
        
        //TODO: Add null check in Db<T>

        public Db(Core core, string dbName, DbType type)
        {
            Core = core;
            DbName = dbName;
            DbType = type;

            Cache = new Dictionary<string, T>();
        }

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

        public T Get(string id)
        {
            return default(T);
        }

        public void Set(T dat)
        {
            //dat.Update(null);
            Core.Db.Set(DbName, dat.Id, Serialize(dat));
        }

    }

    [ProtoContract]
    public class ScriptData : IData
    {
        [ProtoMember(1)]
        public string Id { get; protected set; }

        [ProtoMember(2)]
        public string Type { get; set; }

        [ProtoMember(3)]
        public string Data { get; set; }
#region static
        /// <summary>
        /// Recieve script data from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested sript data or null</returns>
        public static ScriptData GetById(string id)
        {
            using (var file = File.Open("script.bin", FileMode.Open))
            {
                return (ScriptData)Serializer.Deserialize(typeof(ScriptData), file);
            }
        }
        /// <summary>
        /// Send request to the server with request to create new script and to send it's data.
        /// </summary>
        /// <returns>New script data</returns>
        public static ScriptData Create()
        {
            ScriptData r = new ScriptData() { Id = Guid.NewGuid().ToString(), Data = "hello", Type = "test" };
            using (var file = File.Create("script.bin"))
            {
                Serializer.Serialize(file, r);
            }
            return r;
        }

        public void Update(IData data)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    [ProtoContract]
    public class TextData : INotifyPropertyChanged, IData
    {
        [ProtoMember(1)]
        public string Id { get; protected set; }

        private string _text;
        [ProtoMember(2)]
        public string Text {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region static
        /// <summary>
        /// Receive text data from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested text data or null</returns>
        public static TextData GetById(string id)
        {
            using (var file = File.Open("text.bin", FileMode.Open))//вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (TextData)Serializer.Deserialize(typeof(TextData), file);
                
            }
        }

        /// <summary>
        /// Send request to the server with request to create new text and to send it's data.
        /// </summary>
        /// <returns>New text data</returns>
        public static TextData Create()
        {
            TextData r = new TextData() { Id = Guid.NewGuid().ToString(), Text = "hello" };
            using (var file = File.Create("text.bin"))
            {
                Serializer.Serialize(file, r);
            }
            return r;
        }
        #endregion
    }

    [ProtoContract]
    public class DialogData : INotifyPropertyChanged, IData
    {
        #region Members

        [ProtoMember(1)]
        public string Id { get; protected set; }

        private string _name;

        [ProtoMember(2)]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _tags;

        [ProtoMember(3)]
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        private string _description;

        [ProtoMember(4)]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Data can be recieved only with using GetById() or with RecieveData()
        /// </summary>
        [ProtoMember(5, IsRequired = false)]
        public string Data { get; set; }
        //TODO: RecieveData() & SaveData()

        #endregion

        #region notify

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region static

        /// <summary>
        /// Receive all data from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested text data or null</returns>
        public static DialogData GetById(string id)
        {
            using (var file = File.Open("project.bin", FileMode.Open))
                //вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (DialogData) Serializer.Deserialize(typeof (DialogData), file);
            }
        }

        /// <summary>
        /// Receive only Name, Tags & Description from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested text data or null</returns>
        public static DialogData GetPrewiewById(string id)
        {
            using (var file = File.Open("project.bin", FileMode.Open))
                //вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (DialogData) Serializer.Deserialize(typeof (DialogData), file);
            }
        }

        /// <summary>
        /// Send request to the server with request to create new text and to send it's data.
        /// </summary>
        /// <returns>New text data</returns>
        public static DialogData Create()
        {
            DialogData r = new DialogData()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "hello",
                Description = "test project",
                Tags = "test,hello"
            };
            using (var file = File.Create("project.bin"))
            {
                Serializer.Serialize(file, r);
            }
            return r;
        }

        #endregion
    }


    [ProtoContract]
    public class CharactersData : INotifyPropertyChanged, IData
    {
        #region Members

        [ProtoMember(1)]
        public string Id { get; protected set; }

        private string _nameId;
        /// <summary>
        /// ID of TextData from Texts DB. You must get it by yourself. A value of the field can be changed.
        /// </summary>
        [ProtoMember(2)]
        public string NameId
        {
            get { return _nameId; }
            set
            {
                _nameId = value;
                OnPropertyChanged();
            }
        }
        //TODO: Я сомневаюсь насчёт OnPropertyChanged в некоторых местах - решим потом.
        private string _tags;
        [ProtoMember(3)]
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        private string _groups;
        [ProtoMember(4)]
        public string Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        [ProtoMember(5)]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(6, IsRequired = false)]
        public string Sets { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public string Behavior { get; set; }

        [ProtoMember(8, IsRequired = false)]
        public string Knowledge { get; set; }

        #endregion

        #region notify

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region static

        /// <summary>
        /// Receive all data from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested text data or null</returns>
        public static CharactersData GetById(string id)
        {
            using (var file = File.Open("CharactersData.bin", FileMode.Open))
            //вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (CharactersData)Serializer.Deserialize(typeof(CharactersData), file);
            }
        }

        /// <summary>
        /// Receive only Name, Tags & Description from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested text data or null</returns>
        public static CharactersData GetPrewiewById(string id)
        {
            using (var file = File.Open("CharactersData.bin", FileMode.Open))
            //вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (CharactersData)Serializer.Deserialize(typeof(CharactersData), file);
            }
        }

        /// <summary>
        /// Send request to the server with request to create new text and to send it's data.
        /// </summary>
        /// <returns>New text data</returns>
        public static CharactersData Create()
        {
            CharactersData r = new CharactersData()
            {
                Id = Guid.NewGuid().ToString(),
                NameId = "hello",
                Description = "test project",
                Tags = "test,hello"
            };
            using (var file = File.Create("CharactersData.bin"))
            {
                Serializer.Serialize(file, r);
            }
            return r;
        }

        #endregion
    }

}
