using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace EventBuddyPhone
{
    public partial class Events : PhoneApplicationPage
    {
        public Events()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            this.Loaded += Events_Loaded;
        }

        void Events_Loaded(object sender, RoutedEventArgs e)
        {
            string categoryId = this.NavigationContext.QueryString["categoryId"];
            App.ViewModel.LoadCategory(int.Parse(categoryId));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/NewEvent.xaml", UriKind.Relative));
        }

        private void btnSelectEvent_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Event selected = (Event)button.DataContext;
            string uri = string.Format("/EventView.xaml?eventId={0}", selected.Id);
            this.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
    }
}