using GalaSoft.MvvmLight;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.Threading;

namespace EventView.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private bool topmost = true;
        public bool TopMost
        {
            get
            {
                return this.topmost;
            }
            set
            {
                if (this.topmost != value)
                {
                    this.topmost = value;
                    base.RaisePropertyChanged("TopMost");
                }
            }
        }


        private string eventTitle = "N/A";
        public string EventTitle
        {
            get { return this.eventTitle; }
            set
            {
                if (this.eventTitle != value)
                {
                    this.eventTitle = value;
                    base.RaisePropertyChanged("EventTitle");
                }
            }
        }

        private bool connected = true;
        public bool Connected
        {
            get
            {
                return this.connected;
            }
            set
            {
                if (this.connected != value)
                {
                    this.connected = value;
                    base.RaisePropertyChanged("Connected");
                }
            }
        }

        public UserCredential credential { get; set; }
        IList<CalendarListEntry> calendarlist { get; set; }
        public List<Event> EventList { get; set; }

        public List<Event> DayEvent { get; set; }
        public ClientSecrets csecrets = new ClientSecrets();

        public CalendarService calendarservice { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            csecrets.ClientId = "35229438944.apps.googleusercontent.com";
            csecrets.ClientSecret = "8ohxvvPfOTAAAZtaIhUUH2pF";
            Login();
        }

        public async void Login()
        {
            EventTitle = Connected.ToString();
            if (Connected)
            {

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        csecrets,
                        new[] { CalendarService.Scope.Calendar },
                        "user", CancellationToken.None, new FileDataStore("Calendar.Event.View"));

                // Create the service.
                calendarservice = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TitleApp",
                });
                EventTitle = "connected";
            }
            else
            {
                credential = null;
                calendarservice = null;
            }
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}