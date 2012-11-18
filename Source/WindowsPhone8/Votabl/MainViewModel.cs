using Microsoft.Phone.Notification;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
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
        public async Task<Category> LoadCategory(int categoryId)
        {
            var currentCategory = Categories.Single(c => c.Id == categoryId);

            // TODO - load real events
            var events = await App.MobileService.GetTable<Event>().Where(
                e => e.CategoryId == categoryId).ToEnumerableAsync();

            currentCategory.Events.SetRange(events);

            CurrentCategory = currentCategory;
            return currentCategory;
        }

        public async Task<Event> LoadEvent(int eventId)
        {
            var currentEvent = CurrentCategory.Events.Single(e => e.Id == eventId);

            // TODO - actually load votables
            var votables = await App.MobileService.GetTable<Votable>()
                .Where(v => v.EventId == eventId).ToEnumerableAsync();

            currentEvent.Votables.SetRange(votables);

            

            CurrentEvent = currentEvent;
            CurrentEvent.SetMaximum();
            return currentEvent;
        }

        private async Task LoginTwitter()
        {
            // TODO - ensure login via twitter
            await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
        }

        private async Task UploadNotificationChannel()
        {
            var pushChannel = AcquirePushChannel();

            // TODO - upload push channel
            var channel = new JObject();
            channel.Add("channelUri", pushChannel.ChannelUri.AbsoluteUri);
            channel.Add("deviceType", "WP8");

            await App.MobileService.GetTable("channels").InsertAsync(channel);

        }

        private void channel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            string content = new StreamReader(e.Notification.Body).ReadToEnd();
            int eventId = int.Parse(content);

            // TODO - handle push notification
           
            if (App.ViewModel.CurrentEvent == null || App.ViewModel.CurrentEvent.Id != eventId) return;
            _sync.Post(async ignored =>
            {
                await App.ViewModel.LoadEvent(eventId);
            }, null);
             
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
                channel.BindToShellToast();
            }

            channel.HttpNotificationReceived += channel_HttpNotificationReceived;

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

        private bool _initialized = false;

        public async void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _categories.Clear();
            _categories.Add(new Category { Id = 1, Title = "Beer Fest", Image = "http://homebrewfanatic.com/wp-content/uploads/2008/03/beer-fest.jpg", Subtitle = "Enjoy beer at a festival" });
            _categories.Add(new Category { Id = 2, Title = "Garage Sales", Image = "http://bacc.cc/wp-content/uploads/2012/05/garage-sale.jpg", Subtitle = "Buy some trash" });
            _categories.Add(new Category { Id = 3, Title = "Presentations", Image = "http://www.hearsaycommunications.com/images/presentation.jpg", Subtitle = "Watch a cool presentation" });

            await LoginTwitter();

            await UploadNotificationChannel();
        }

        private SynchronizationContext _sync = SynchronizationContext.Current;
    }
}
