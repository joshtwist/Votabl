using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBuddyPhone
{
    public class Event : ViewModel
    {
        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _title;

        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref _title, value); }
        }

        private string _subtitle;

        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref _subtitle, value); }
        }

        private int _categoryId;

        public int CategoryId
        {
            get { return this._categoryId; }
            set { this.SetProperty(ref _categoryId, value); }
        }

        private int _maximum;

        public int Maximum
        {
            get { return this._maximum; }
            set { this.SetProperty(ref _maximum, value); }
        }

        private ObservableCollection<Votable> _votables = new ObservableCollection<Votable>();

        public ObservableCollection<Votable> Votables
        {
            get { return this._votables; }
            set { this.SetProperty(ref _votables, value); }
        }

        internal void SetMaximum()
        {
            if (Votables.Count == 0) return;
            Maximum = Votables.Max(v => v.Count);
            Votables.SortDesc(v => v.Count);
        }
    }

    public class Channel : ViewModel
    {
        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _channelUri;

        public string ChannelUri
        {
            get { return this._channelUri; }
            set { this.SetProperty(ref _channelUri, value); }
        }

        private string _deviceType = "WP8";

        public string DeviceType
        {
            get { return this._deviceType; }
            set { this.SetProperty(ref _deviceType, value); }
        }
    }

    public class Category : ViewModel
    {
        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _title;

        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref _title, value); }
        }

        private string _subtitle;

        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref _subtitle, value); }
        }

        private string _image;

        public string Image
        {
            get { return this._image; }
            set { this.SetProperty(ref _image, value); }
        }

        private ObservableCollection<Event> _events = new ObservableCollection<Event>();

        public ObservableCollection<Event> Events
        {
            get { return this._events; }
            set { this.SetProperty(ref _events, value); }
        }
    }

    public class Votable : ViewModel
    {
        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _name;

        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref _name, value); }
        }

        private int _eventId;

        public int EventId
        {
            get { return this._eventId; }
            set { this.SetProperty(ref _eventId, value); }
        }

        private int _count;

        [DataMemberJsonConverter(ConverterType = typeof(OneWayConverter))]
        public int Count
        {
            get { return this._count; }
            set { this.SetProperty(ref _count, value); }
        }
    }

    public class OneWayConverter : IDataMemberJsonConverter
    {
        public object ConvertFromJson(JToken value)
        {
            return value.ToObject<object>();
        }

        JToken IDataMemberJsonConverter.ConvertToJson(object instance)
        {
            return null;
        }
    }
}
