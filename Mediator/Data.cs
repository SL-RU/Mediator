using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Mediator
{
    [ProtoContract]
    public class DataVersionInfo
    {
        [ProtoMember(1)]
        public string Version = "";

        [ProtoMember(2)]
        public string LastEditBy = "";

        [ProtoMember(3)]
        public string LastEditTime = "";
    }

    public interface IData
    {
        //Weeell, let client be creating new data.
        DataVersionInfo Version { get; }
        string Id { get; set; }
        bool IsFullDataRecieved { get; }
    }

    [ProtoContract]
    public class Data : IData
    {
        private byte[] _serializedVersion;
        private DataVersionInfo _dataVersion;
        [ProtoMember(1)]
        public byte[] SerializedVersion
        {
            get { return _serializedVersion; }
            set
            {
                _serializedVersion = value;
                if (_serializedVersion != null)
                {
                    using (var m = new MemoryStream(_serializedVersion))
                    {
                        _dataVersion = (DataVersionInfo)Serializer.Deserialize(typeof(DataVersionInfo), m);
                    }
                }
                else
                {
                    _dataVersion = null;
                }
            }
        }
        public DataVersionInfo Version
        {
            get
            {
                if (_dataVersion != null)
                    return _dataVersion;
                if (SerializedVersion != null)
                {
                    using (var m = new MemoryStream(SerializedVersion))
                    {
                        _dataVersion = (DataVersionInfo)Serializer.Deserialize(typeof(DataVersionInfo), m);
                    }
                    return _dataVersion;
                }
                return null;
            }
        }

        public string Id { get; set; }
        public bool IsFullDataRecieved => true;
    }

    [ProtoContract]
    public class ScriptData : Data
    {
        [ProtoMember(2)]
        public string Type { get; set; }

        [ProtoMember(3)]
        public byte[] Data { get; set; }
    }

    [ProtoContract]
    public class TextData : Data, INotifyPropertyChanged
    {
        private string _text;
        [ProtoMember(2)]
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [ProtoContract]
    public class DialogData : Data, INotifyPropertyChanged
    {
        #region Members
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
        public byte[] Data { get; set; }

        public new bool IsFullDataRecieved => Data != null;
        #endregion

        #region notify

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [ProtoContract]
    public class CharacterData : Data, INotifyPropertyChanged
    {
        #region Members
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
        public byte[] Sets { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public byte[] Behavior { get; set; }

        [ProtoMember(8, IsRequired = false)]
        public byte[] Knowledge { get; set; }

        public new bool IsFullDataRecieved => Behavior != null;
        #endregion

        #region notify

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
