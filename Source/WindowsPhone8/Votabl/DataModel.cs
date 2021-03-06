﻿using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventBuddyPhone
{
    [DataTable(Name = "userCategories")]
    public class UserCategory
    {
        public int Id { get; set; }

        [DataMember(Name = "categoryId")]
        public int CategoryId { get; set; }

        [DataMember(Name = "userId")]
        public string UserId { get; set; }
    }

    [DataTable(Name = "events")]
    public class Event : ViewModel
    {
        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _title;

        [DataMember(Name = "title")]
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref _title, value); }
        }

        private string _subtitle;

        [DataMember(Name = "subtitle")]
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref _subtitle, value); }
        }

        private int _categoryId;

        [DataMember(Name = "categoryId")]
        public int CategoryId
        {
            get { return this._categoryId; }
            set { this.SetProperty(ref _categoryId, value); }
        }

        private ObservableCollection<Votable> _votables = new ObservableCollection<Votable>();

        [IgnoreDataMember]
        public ObservableCollection<Votable> Votables
        {
            get { return this._votables; }
            set { this.SetProperty(ref _votables, value); }
        }

        // TODO - add maximum count property
        private int _maximum;

        public int Maximum
        {
            get { return this._maximum; }
            set { this.SetProperty(ref _maximum, value); }
        }


        internal void SetMaximum()
        {
            if (Votables.Count == 0) {
                Maximum = 0;
                return;
            }

            Maximum = Votables.Max(v => v.Count);
            Votables.SortDesc(v => v.Count);
        }
    }

    [DataTable(Name = "votables")]
    public class Votable : ViewModel
    {
        private int _id;

        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _name;

        [DataMember(Name = "name")]
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref _name, value); }
        }

        private int _eventId;

        [DataMember(Name = "eventId")]
        public int EventId
        {
            get { return this._eventId; }
            set { this.SetProperty(ref _eventId, value); }
        }

        // TODO - add count property

        private int _count;

        [DataMember(Name = "count")]
        public int Count
        {
            get { return this._count; }
            set { this.SetProperty(ref _count, value); }
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


}
