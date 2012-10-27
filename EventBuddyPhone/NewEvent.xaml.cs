using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace EventBuddyPhone
{
    public partial class NewEvent : PhoneApplicationPage
    {
        public NewEvent()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }

        private void btnAddVotable_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtVotable.Text))
            {
                return;
            }
            listVotables.Items.Add(txtVotable.Text);
            txtVotable.Text = string.Empty;
            txtVotable.Focus();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private async void btnSave_Click_1(object sender, EventArgs e)
        {
            // TODO - save new event and votables


            this.NavigationService.GoBack();
        }


    }
}