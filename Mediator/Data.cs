using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Mediator
{
    public interface IData
    {
        string Id { get; }
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
    }

    [ProtoContract]
    public class TextData : INotifyPropertyChanged, IData
    {
        [ProtoMember(1)]
        public string Id { get; protected set; }

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
    }
}
