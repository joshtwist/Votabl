﻿using System;
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

        private async void btnSave_Click_1(object sender, EventArgs e)
        {
            // TODO - save data
            var newEvent = new Event
            {
                CategoryId = App.ViewModel.CurrentCategory.Id,
                Title = txtTitle.Text,
                Subtitle = txtSubtitle.Text
            };
            await App.MobileService.GetTable<Event>().InsertAsync(newEvent);

            foreach (var votableName in listVotables.Items.OfType<String>())
            {
                var votable = new Votable {
                    EventId = newEvent.Id,
                    Name = votableName
                };
                await App.MobileService.GetTable<Votable>().InsertAsync(votable);
            }

            this.NavigationService.GoBack();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}