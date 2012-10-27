using Microsoft.Phone.Notification;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventBuddyPhone
{
    public class MainViewModel : ViewModel
    {
        private SynchronizationContext _sync = SynchronizationContext.Current;

        private async Task LoginTwitter()
        {
            await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
        }

        private async Task UploadNotificationChannel()
        {
            var pushChannel = AcquirePushChannel();
            var channel = new Channel
            {
                ChannelUri = pushChannel.ChannelUri.AbsoluteUri
            };
            await App.MobileService.GetTable<Channel>().InsertAsync(channel);
        }

        private async void channel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            string content = new StreamReader(e.Notification.Body).ReadToEnd();
            int eventId = int.Parse(content);
            if (App.ViewModel.CurrentEvent.Id != eventId) return;
            await App.ViewModel.LoadEvent(eventId);
        }

        public static HttpNotificationChannel CurrentChannel { get; private set; }

        private HttpNotificationChannel AcquirePushChannel()
        {
            var channel = HttpNotificationChannel.Find("MyPushChannel");

            if (channel == null)
            {
                channel = new HttpNotificationChannel("MyPushChannel");
                channel.Open();
                channel.BindToShellTile();
                channel.HttpNotificationReceived += channel_HttpNotificationReceived;
            }

            return channel;   
        }

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();

        public ObservableCollection<Category> Categories
        {
            get { return this._categories; }
            set { this.SetProperty(ref _categories, value); }
        }

        private Category _currentCategory;

        public Category CurrentCategory
        {
            get { return this._currentCategory; }
            set { this.SetProperty(ref _currentCategory, value); }
        }

        private Event _currentEvent;

        public Event CurrentEvent
        {
            get { return this._currentEvent; }
            set { this.SetProperty(ref _currentEvent, value); }
        }

        public async Task<Category> LoadCategory(int categoryId)
        {
            var currentCategory = Categories.Single(c => c.Id == categoryId);
            // TODO - load events
            //currentCategory.Events.Clear();
            //currentCategory.Events.Add(new Event
            //{
            //    Title = "Event Title",
            //    Subtitle = "Event Subtitle"
            //});

            var events = await App.MobileService.GetTable<Event>()
                .Where(e => e.CategoryId == categoryId).ToEnumerableAsync();
            currentCategory.Events.SetRange(events);
            CurrentCategory = currentCategory;
            return currentCategory;
        }

        public async Task<Event> LoadEvent(int eventId)
        {
            var currentEvent = CurrentCategory.Events.Single(e => e.Id == eventId);
            // TODO - actually load votables
            //currentEvent.Votables.Clear();
            //currentEvent.Votables.SetRange(Enumerable.Range(0,12).Select(i => new Votable
            //{
            //    EventId = currentEvent.Id,
            //    Name = "Votable " + i.ToString()
            //}));

            var votables = await App.MobileService.GetTable<Votable>()
                .Where(v => v.EventId == eventId).ToEnumerableAsync();
            currentEvent.Votables.SetRange(votables);
            currentEvent.SetMaximum();
            CurrentEvent = currentEvent;
            return currentEvent;
        }

        private bool _initialized = false;

        public async void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _categories.Clear();
            _categories.Add(new Category { Id = 1, Title = "Beer Fest", Image = "http://homebrewfanatic.com/wp-content/uploads/2008/03/beer-fest.jpg", Subtitle = "Enjoy beer at a festival" });
            _categories.Add(new Category { Id = 2, Title = "Garage Sales", Image = "http://bacc.cc/wp-content/uploads/2012/05/garage-sale.jpg", Subtitle = "Buy some crap" });
            _categories.Add(new Category { Id = 3, Title = "Presentations", Image = "http://www.hearsaycommunications.com/images/presentation.jpg", Subtitle = "Enjoy beer at a festival" });

            if (App.MobileService.CurrentUser == null)
            {
                await LoginTwitter();
            }

            await UploadNotificationChannel();

            
        }
    }
}
