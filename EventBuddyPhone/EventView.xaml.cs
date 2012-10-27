﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;

namespace EventBuddyPhone
{
    public partial class EventView : PhoneApplicationPage
    {
        public EventView()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            this.Loaded += EventView_Loaded;
        }

        public async void EventView_Loaded(object sender, RoutedEventArgs e)
        {
            string eventId = this.NavigationContext.QueryString["eventId"];
            await App.ViewModel.LoadEvent(int.Parse(eventId));
        }

        private async void btnVote_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            button.IsEnabled = false;
            Votable votable = (Votable)button.DataContext;

            // TODO submit the vote
            JObject vote = new JObject();
            vote.Add("EventId", votable.EventId);
            vote.Add("VotableId", votable.Id);

            await App.MobileService.GetTable("Vote").InsertAsync(vote);
            button.IsEnabled = true;
        }
    }
}