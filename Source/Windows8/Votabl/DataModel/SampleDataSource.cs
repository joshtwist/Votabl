using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using System.Threading.Tasks;
using EventBuddy.Common;
using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace EventBuddy.Data
{
    /// <summary>
    /// Base class for <see cref="Event"/> and <see cref="Category"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : EventBuddy.Common.BindableBase
    {
        private int _id;
        public int Id
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private string _title = string.Empty;
        
        [DataMember(Name = "title")]
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;

        [DataMember(Name = "subtitle")]
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    [DataTable(Name = "channels")]
    public class Channel
    {
        public Channel()
        {
            DeviceType = "W8";
        }

        public string Id { get; set; }

        [DataMember(Name = "channelUri")]
        public string ChannelUri { get; set; }

        [DataMember(Name = "deviceType")]
        public string DeviceType { get; set; }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    [DataTable(Name = "events")]
    public class Event : SampleDataCommon
    {
        private int _categoryId;

        [DataMember(Name = "categoryId")]
        public int CategoryId
        {
            get { return this._categoryId; }
            set { this.SetProperty(ref _categoryId, value); }
        }

        private readonly ObservableCollection<Votable> _votables = new ObservableCollection<Votable>();

        [IgnoreDataMember]
        public ObservableCollection<Votable> Votables
        {
            get { return _votables; }
        }

        private int _maximum;

        [IgnoreDataMember]
        public int Maximum
        {
            get { return this._maximum; }
            set { this.SetProperty(ref _maximum, value); }
        }

        internal void CalculateMaximum()
        {
            if (Votables.Count == 0){
                Maximum = 0;
            }
            else
            {
                Maximum = Votables.Max(v => v.Count);
                Votables.SortDesc(v => v.Count);
            }
        }
    }

    [DataTable(Name = "votables")]
    public class Votable : BindableBase
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

        private int _count;

        [DataMemberJsonConverter(ConverterType = typeof(OneWayConverter))]
        [DataMember(Name = "count")]
        public int Count
        {
            get { return this._count; }
            set { this.SetProperty(ref _count, value); }
        }
    }

    [DataTable(Name = "votes")]
    public class Vote
    {
        public int Id { get; set; }

        [DataMember(Name = "votableId")]
        public int VotableId { get; set; }

        [DataMember(Name = "eventId")]
        public int EventId { get; set; }
    }

    [DataTable(Name = "userCategories")]
    public class UserCategory
    {
        public int Id { get; set; }

        [DataMember(Name = "categoryId")]
        public int CategoryId { get; set; }
    }

    public class OneWayConverter : IDataMemberJsonConverter
    {
        public object ConvertFromJson(Windows.Data.Json.IJsonValue value)
        {
            switch (value.ValueType)
            {
                case Windows.Data.Json.JsonValueType.Array:
                    return value.GetArray();
                case Windows.Data.Json.JsonValueType.Boolean:
                    return value.GetBoolean();
                case Windows.Data.Json.JsonValueType.Null:
                    return null;
                case Windows.Data.Json.JsonValueType.Number:
                    return value.GetNumber();
                case Windows.Data.Json.JsonValueType.Object:
                    return value.GetObject();
                case Windows.Data.Json.JsonValueType.String:
                    return value.GetString();
                default:
                    return null;
            }
        }

        public Windows.Data.Json.IJsonValue ConvertToJson(object instance)
        {
            // Just send null
            return null;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class Category : SampleDataCommon
    {
        public Category()
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private string _image;

        public string Image
        {
            get { return this._image; }
            set { this.SetProperty(ref _image, value); }
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<Event> _items = new ObservableCollection<Event>();
        public ObservableCollection<Event> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<Event> _topItem = new ObservableCollection<Event>();
        public ObservableCollection<Event> TopItems
        {
            get { return this._topItem; }
        }
    }




    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<Category> _allGroups = new ObservableCollection<Category>();
        public ObservableCollection<Category> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<Category> GetCategories()
        {
            return _sampleDataSource.AllGroups;
        }

        public static async Task<Category> GetCategory(int uniqueId)
        {
            var category = _sampleDataSource.AllGroups.Single(c => c.Id == uniqueId);
            // TODO - load the events into category.Items
            var events = await App.MobileService.GetTable<Event>().Where(e => e.CategoryId == uniqueId).ToEnumerableAsync();
            category.Items.SetItems(events);
            return category;
        }

        public static Event GetItem(string uniqueId)
        {
            // Not used in the demo
            return null;
        }

        public SampleDataSource()
        {
            _allGroups.Add(new Category { Id = 1, Title = "Beer Fest", Image = "http://homebrewfanatic.com/wp-content/uploads/2008/03/beer-fest.jpg", Subtitle = "Enjoy beer at a festival" });
            _allGroups.Add(new Category { Id = 2, Title = "Garage Sales", Image = "http://bacc.cc/wp-content/uploads/2012/05/garage-sale.jpg", Subtitle = "Buy some trash" });
            _allGroups.Add(new Category { Id = 3, Title = "Presentations", Image = "http://www.hearsaycommunications.com/images/presentation.jpg", Subtitle = "Watch a cool presentation" });
        }
    }
}
