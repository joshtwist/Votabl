using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using EventBuddyPhone.Resources;

namespace EventBuddyPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            DataContext = App.ViewModel;
            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.Initialize();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Category c = (Category) b.DataContext;
            string uri = string.Format("/Events.xaml?categoryId={0}", c.Id);
            this.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

        private void chkSubscribe_Checked_1(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Category cat = (Category)cb.DataContext;

            CategoryCheckBox ccb = new CategoryCheckBox { Category = cat, CheckBox = cb };

            if (cb.IsChecked.GetValueOrDefault() && !_subscriptions.Contains(ccb))
            {
                _subscriptions.Add(ccb);
            }
            else if (!cb.IsChecked.GetValueOrDefault() && _subscriptions.Contains(ccb))
            {
                _subscriptions.Remove(ccb);
            }
        }

        class CategoryCheckBox
        {
            public Category Category { get; set; }
            public CheckBox CheckBox { get; set; }

            public override bool Equals(object obj)
            {
                var target = obj as CategoryCheckBox;
                if (target == null)
                {
                    return false;
                }
                return Object.Equals(Category, target.Category);
            }
        }

        private List<CategoryCheckBox> _subscriptions = new List<CategoryCheckBox>();

        private async void btnSubscribe_Click_1(object sender, EventArgs e)
        {
            // TODO - subscribe to events by enumerating _subscriptions collection
            // and uncheck the checkboxes
            foreach (var sub in _subscriptions.ToArray())
            {
                var uc = new UserCategory
                {
                    CategoryId = sub.Category.Id,
                    UserId = App.MobileService.CurrentUser.UserId
                };

                await App.MobileService.GetTable<UserCategory>().InsertAsync(uc);
                sub.CheckBox.IsChecked = false;
                _subscriptions.Remove(sub);
            }

        }
    }
}