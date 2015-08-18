using HubApp1.Common;
using HubApp1.Data;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace HubApp1
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        IHubProxy chat;
        Subscription subscriptio = new Subscription();
        public SynchronizationContext Context { get; set; }
        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }
      
       
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!enterName.IsOpen)
            {
                border.Width = Window.Current.Bounds.Width;
                border.Height = Window.Current.Bounds.Height;
                enterName.IsOpen = true;
                chatName.Focus(FocusState.Pointer);
            }
        }

       
        async private void makeConnection()
        {
            try
            {
                // Pass parameter form client to server.
                var querystringData = new Dictionary<string, string>();
                querystringData.Add("chatversion", "1.1");

                var hubConnection = new HubConnection("http://localhost:53748",querystringData);
               
                // Windows authentication.
                    hubConnection.Credentials = CredentialCache.DefaultCredentials;
                // Connection Header.
              //  hubConnection.Headers.Add("myauthtoken", /* token data */);
                //// Auth(Certificate).
                //hubConnection.AddClientCertificate(X509Certificate.CreateFromCertFile("MyCert.cer"));

                chat = hubConnection.CreateHubProxy("ChatHub");
                chatShow.Text = "";
                Context = SynchronizationContext.Current;
                
                chat.On<string, string>("broadcastMessage",
                    (name, message) =>
                        Context.Post(delegate
                        {
                            this.chatShow.Text += name + ": "; this.chatShow.Text += message + "\n";
                        }, null)
                        );
              
                chat.Subscribe("notifyWrongVersion").Received += notifyWrongVersion_chat;
               

                
                await hubConnection.Start();
                await chat.Invoke("Notify", chatName.Text, hubConnection.ConnectionId);
            }
            catch (HubException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }
    
        public void notifyWrongVersion()
        {

        }
        async void notifyWrongVersion_chat(IList<Newtonsoft.Json.Linq.JToken> obj)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                notifyWrongVersion();
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        async private void sendMessage()
        {
            try
            {
                await chat.Invoke("Send", chatName.Text, txtMessage.Text);
            }
            catch (HubException ex)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (enterName.IsOpen)
            {
                enterName.IsOpen = false;
            }
            makeConnection();
        } 
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
