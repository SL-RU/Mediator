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
    public interface IData
    {
        //Weeell, let client be creating new data.
        string Locked { get; }
        string Version { get; }
        string EditBy { get; }
        string EditTime { get; }

        string Id { get; set; }
        bool IsFullDataRecieved { get; }
    }

    public class Data : IData, INotifyPropertyChanged
    {

        public string Id { get; set; }
        public virtual bool IsFullDataRecieved => true;

        public virtual string Locked { get; set; }
        public virtual string Version { get; set; }
        public virtual string EditBy { get; set; }
        public virtual string EditTime { get; set; }

        #region notify

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [ProtoContract]
    public class ScriptData : Data, INotifyPropertyChanged
    {
        #region kostyl' because of protobuf

        private string _locked;

        [ProtoMember(1)]
        public override string Locked
        {
            get { return _locked; }
            set
            {
                _locked = value;
                OnPropertyChanged();
            }
        }

        private string _version;

        [ProtoMember(2)]
        public override string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private string _editBy;

        [ProtoMember(3)]
        public override string EditBy
        {
            get { return _editBy; }
            set
            {
                _editBy = value;
                OnPropertyChanged();
            }
        }

        private string _editTime;

        [ProtoMember(4)]
        public override string EditTime
        {
            get { return _editTime; }
            set
            {
                _editTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        [ProtoMember(5)]
        public string Type { get; set; }

        [ProtoMember(6)]
        public byte[] Data { get; set; }
    }

    [ProtoContract]
    public class TextData : Data, INotifyPropertyChanged
    {
        #region kostyl' because of protobuf

        private string _locked;

        [ProtoMember(1)]
        public override string Locked
        {
            get { return _locked; }
            set
            {
                _locked = value;
                OnPropertyChanged();
            }
        }

        private string _version;

        [ProtoMember(2)]
        public override string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private string _editBy;

        [ProtoMember(3)]
        public override string EditBy
        {
            get { return _editBy; }
            set
            {
                _editBy = value;
                OnPropertyChanged();
            }
        }

        private string _editTime;

        [ProtoMember(4)]
        public override string EditTime
        {
            get { return _editTime; }
            set
            {
                _editTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        private string _text;

        [ProtoMember(5)]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
    }

    [ProtoContract]
    public class DialogData : Data, INotifyPropertyChanged
    {
        #region kostyl' because of protobuf

        private string _locked;

        [ProtoMember(1)]
        public override string Locked
        {
            get { return _locked; }
            set
            {
                _locked = value;
                OnPropertyChanged();
            }
        }

        private string _version;

        [ProtoMember(2)]
        public override string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private string _editBy;

        [ProtoMember(3)]
        public override string EditBy
        {
            get { return _editBy; }
            set
            {
                _editBy = value;
                OnPropertyChanged();
            }
        }

        private string _editTime;

        [ProtoMember(4)]
        public override string EditTime
        {
            get { return _editTime; }
            set
            {
                _editTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Members

        private string _name;

        [ProtoMember(5)]
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

        [ProtoMember(6)]
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

        [ProtoMember(7)]
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
        [ProtoMember(8, IsRequired = false)]
        public byte[] Data { get; set; }

        public new bool IsFullDataRecieved => Data != null;

        #endregion
    }

    [ProtoContract]
    public class CharacterData : Data, INotifyPropertyChanged
    {
        #region kostyl' because of protobuf

        private string _locked;

        [ProtoMember(1)]
        public override string Locked
        {
            get { return _locked; }
            set
            {
                _locked = value;
                OnPropertyChanged();
            }
        }

        private string _version;

        [ProtoMember(2)]
        public override string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private string _editBy;

        [ProtoMember(3)]
        public override string EditBy
        {
            get { return _editBy; }
            set
            {
                _editBy = value;
                OnPropertyChanged();
            }
        }

        private string _editTime;

        [ProtoMember(4)]
        public override string EditTime
        {
            get { return _editTime; }
            set
            {
                _editTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Members

        private string _nameId;

        /// <summary>
        /// ID of TextData from Texts DB. You must get it by yourself. A value of the field can be changed.
        /// </summary>
        [ProtoMember(5)]
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

        [ProtoMember(6)]
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

        [ProtoMember(7)]
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

        [ProtoMember(8)]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        [ProtoMember(9, IsRequired = false)]
        public byte[] Sets { get; set; }

        [ProtoMember(10, IsRequired = false)]
        public byte[] Behavior { get; set; }

        [ProtoMember(11, IsRequired = false)]
        public byte[] Knowledge { get; set; }

        public new bool IsFullDataRecieved => Behavior != null;

        #endregion
    }
}
