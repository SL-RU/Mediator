using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace Mediator
{
    public enum Db
    {
        Scripts,
        Texts,
        Characters,
        Projects,
        Quest
    }

    public class Core
    {
        public static void Init()
        {

        }

        public static string ReceiveData(Db db, string id)
        {
            return "";
        }

    }

    [ProtoContract]
    public class ScriptData
    {
        [ProtoMember(1)]
        public string ID { get; set; }

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
            ScriptData r = new ScriptData() { ID = Guid.NewGuid().ToString(), Data = "hello", Type = "test" };
            using (var file = File.Create("script.bin"))
            {
                Serializer.Serialize(file, r);
            }
            return r;
        }
#endregion
    }

    [ProtoContract]
    public class TextData : INotifyPropertyChanged
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
    public class ProjectData : INotifyPropertyChanged
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
        public static ProjectData GetById(string id)
        {
            using (var file = File.Open("project.bin", FileMode.Open))
                //вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (ProjectData) Serializer.Deserialize(typeof (ProjectData), file);
            }
        }

        /// <summary>
        /// Receive only Name, Tags & Description from server with id.
        /// </summary>
        /// <param name="id">requested id</param>
        /// <returns>Requested text data or null</returns>
        public static ProjectData GetPrewiewById(string id)
        {
            using (var file = File.Open("project.bin", FileMode.Open))
                //вместо етого вызываем чёт типа Core.RequestData(DB.Texts, id);
            {
                return (ProjectData) Serializer.Deserialize(typeof (ProjectData), file);
            }
        }

        /// <summary>
        /// Send request to the server with request to create new text and to send it's data.
        /// </summary>
        /// <returns>New text data</returns>
        public static ProjectData Create()
        {
            ProjectData r = new ProjectData()
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
    public class CharactersData : INotifyPropertyChanged
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
