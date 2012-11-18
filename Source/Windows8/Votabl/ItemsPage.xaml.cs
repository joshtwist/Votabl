using EventBuddy.Data;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace EventBuddy
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : EventBuddy.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            while (App.MobileService.CurrentUser == null)
            {
                await App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);
            }

            var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += channel_PushNotificationReceived;
            App.MobileService.GetTable<Channel>().InsertAsync(new Channel { ChannelUri = channel.Uri });

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = SampleDataSource.GetCategories();
            this.DefaultViewModel["Items"] = sampleDataGroups;
        }

        async void channel_PushNotificationReceived(Windows.Networking.PushNotifications.PushNotificationChannel sender, Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs args)
        {
            if (args.NotificationType != Windows.Networking.PushNotifications.PushNotificationType.Raw)
            {
                return;
            }

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                int eventId = int.Parse(args.RawNotification.Content);
                var cat = SampleDataSource.GetCategories().SingleOrDefault(c => c.Items.Any(x => x.Id == eventId));
                if (cat == null) return;
                var evt = cat.Items.Where(x => x.Id == eventId).FirstOrDefault();
                if (evt == null) return;
                var votes = await App.MobileService.GetTable<Votable>().Where(v => v.EventId == eventId).ToEnumerableAsync();
                foreach (var vote in votes)
                {
                    evt.Votables.Single(v => v.Id == vote.Id).Count = vote.Count;
                }
                evt.CalculateMaximum();
            });
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var groupId = ((Category)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(SplitPage), groupId);
        }

        private void btnAddNew_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewEvent));
        }

        private async void btnSubscribe_Click_1(object sender, RoutedEventArgs e)
        {
            var selected = itemGridView.SelectedItems.OfType<Category>().ToArray();

            foreach (var category in selected)
            {
                await App.MobileService.GetTable<UserCategory>().InsertAsync(new UserCategory
                {
                    CategoryId = category.Id
                });

                itemGridView.SelectedItems.Remove(category);
            }
        }
    }
}
