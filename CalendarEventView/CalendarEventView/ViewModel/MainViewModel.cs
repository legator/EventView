using CalendarEventView.Model;
using GalaSoft.MvvmLight;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace CalendarEventView.ViewModel
{
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

        private string titleapp = "Calendar Event View";
        public string TitleApp
        {
            get { return this.titleapp; }
            set
            {
                if (this.titleapp != value)
                {
                    this.titleapp = value;
                    base.RaisePropertyChanged("TitleApp");
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

        private string eventtitle = "N/A";
        public string EventTitle
        {
            get { return this.eventtitle; }
            set
            {
                if (this.eventtitle != value)
                {
                    this.eventtitle = value;
                    base.RaisePropertyChanged("EventTitle");
                }
            }
        }

        private string timetievent = "";
        public string TimeToEvent
        {
            get { return this.timetievent; }
            set
            {
                if (this.timetievent != value)
                {
                    this.timetievent = value;
                    base.RaisePropertyChanged("TimeToEvent");
                }
            }
        }

        private string dayeventint = "0";
        public string DayEventInt
        {
            get { return this.dayeventint; }
            set
            {
                if (this.dayeventint != value)
                {
                    this.dayeventint = value;
                    base.RaisePropertyChanged("DayEventInt");
                }
            }
        }

        private string temperature = "0*C";
        public string Temparature
        {
            get { return this.temperature; }
            set
            {
                if (this.temperature != value)
                {
                    this.temperature = value;
                    base.RaisePropertyChanged("Temparature");
                }
            }
        }

        private ImageSource weathericon = null;
        public ImageSource WeatherIcon
        {
            get { return this.weathericon; }
            set
            {
                if (this.weathericon != value)
                {
                    this.weathericon = value;
                    base.RaisePropertyChanged("WeatherIcon");
                }
            }
        }

        public Weather Weather { get; set; }
        public string LocationCode { get; set; }
        public string CalendarID { get; set; }
        /// <summary>
        /// /
        /// </summary>
        public int image { get; set; }
        public string simage { get; set; }
        public string skycode { get; set; }
        public string str { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public UserCredential credential { get; set; }
        IList<Google.Apis.Calendar.v3.Data.CalendarListEntry> calendarlist { get; set; }
        public List<Event> EventList = new List<Event>();

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
            Weather = new Weather();
            LocationCode = CalendarEventView.Properties.Settings.Default.Location;
            CalendarID = CalendarEventView.Properties.Settings.Default.CalendarID;
        }

        public async void Log()
        {
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
                    ApplicationName = TitleApp,
                });
                await GetCalendarList();
            }
            else
            {
                credential = null;
                calendarservice = null;
            }
        }

        public async Task GetCalendarList()
        {
            calendarlist = (await calendarservice.CalendarList.List().ExecuteAsync()).Items;
            GetAllEvent();
        }

        public async Task GetAllEvent()
        {
            CalendarListEntry defaultcalendar = new CalendarListEntry();

            if (String.IsNullOrEmpty(CalendarID))
            {
                defaultcalendar = calendarlist[0];
            }
            else
            {
                foreach (var item in calendarlist)
                {
                    if (CalendarID==item.Id)
                    {
                        defaultcalendar = item;
                    }
                }
            }
            
            EventTitle = defaultcalendar.Id;
            
            if (defaultcalendar!=null)
	        {
                //all event

                var calEvents = await calendarservice.Events.List(defaultcalendar.Id).ExecuteAsync();
                AddEvent(calEvents);
                var nextPage = calEvents.NextPageToken;
                while (nextPage != null)
                {
                    var listRequest = calendarservice.Events.List(defaultcalendar.Id);
                    listRequest.PageToken = nextPage;
                    calEvents = await listRequest.ExecuteAsync();
                    AddEvent(calEvents);
                    nextPage = calEvents.NextPageToken;
                }
                /*
                //instance event
                foreach (Event item in EventList)
                {
                    Events res = await calendarservice.Events.Instances(defaultcalendar.Id, item.Id).ExecuteAsync();
                    try
                    {
                        foreach (Event it in res.Items)
                        {
                            //evt.Add(it);
                            EventList.Add(it);
                        }
                    }
                    catch (Exception) { }
                }
                */
                DayEventInt = EventList.Count.ToString();
	        }

            DateTime dt = new DateTime();
            dt = DateTime.Now;
            DayEvents(dt);
        }

        public void AddEvent(Events evs)
        {
            foreach (Event item in evs.Items)
            {
                EventList.Add(item);
            }
        }

        public async void DayEvents(DateTime dt)
        {
            List<Event> evt = new List<Event>();
            foreach (Event item in EventList)
            {
                if (item.Start.DateTime != null)
                {
                    if (item.Start.DateTime.Value.ToShortDateString() == dt.ToShortDateString())
                    {
                        int i = DateTime.Compare(item.Start.DateTime.Value,dt);
                        if(i>0)
                            evt.Add(item);
                    }
                }
            }

            DayEvent = evt;
            DayEventInt = DayEvent.Count.ToString();
            if (DayEvent.Count!=0)
            {
                FirstEvent();
            }
            else
            {
                EventTitle = "No event for this day";
                DayEventInt = "0";
                TimeToEvent = "N/A";
            }
            
        }

        public async void FirstEvent()
        {
            Event early = new Event();
            early = DayEvent[0];
            for (int i = 1; i < DayEvent.Count; i++)
            {
                int j = DateTime.Compare(early.Start.DateTime.Value, DayEvent[i].Start.DateTime.Value);
                if (j>=0)
                {
                    early = DayEvent[i];
                }
            }
            double minutes = (early.Start.DateTime.Value - DateTime.Now).TotalMinutes;
            int min = Convert.ToInt32(minutes.ToString().Split(',')[0]);
            TimeToEvent = min.ToString();
            EventTitle = early.Summary;
        }

        public async void GetWeather()
        {
            string url = string.Format(@"http://weather.service.msn.com/data.aspx?culture=en-US&wealocations={0}&weadegreetype=C", LocationCode);
            HttpWebRequest request2 = WebRequest.CreateHttp(url);
            WebResponse responseAsync = request2.GetResponse();
            WebResponse webResponse = responseAsync;
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string endAsync = streamReader.ReadToEnd();
            str = endAsync;

            XDocument doc = XDocument.Parse(str);
            foreach (XElement element in doc.Descendants("weather"))
            {
                Weather.TimeZone = element.Attribute("timezone").Value;
                Weather.LocationCode = element.Attribute("weatherlocationcode").Value;
                Weather.LocationName = element.Attribute("weatherlocationname").Value;
            }
            foreach (XElement element in doc.Descendants("current"))
            {
                Weather.Temperature.CurrentValue = element.Attribute("temperature").Value+"*C";
                Temparature = Weather.Temperature.CurrentValue;
                Weather.Temperature.FeelsLike = element.Attribute("feelslike").Value + "*C";
                Weather.Temperature.SkyCode = element.Attribute("skycode").Value;
                skycode = element.Attribute("skycode").Value;
                Weather.Temperature.SkyText = element.Attribute("skytext").Value;
                Weather.Temperature.Wind = element.Attribute("winddisplay").Value;
            }
            image = await GetWeatherIcon();
            simage = image.ToString();
            if (simage.Length==1)
	        {
		        simage = "0" + simage;
	        }
            var source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(string.Format("/CalendarEventView;component/images/Weather/weather_vectorgraphic_light_l_{0}.png", simage), UriKind.Relative);
            source.EndInit();
            WeatherIcon = source;
        }

        private async Task<int> GetWeatherIcon()
        {
            int sunset = 19;
            int sunrise = 6;
            switch (Convert.ToInt32(Weather.Temperature.SkyCode))
            {
                case 26:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 34;
                    else
                        return 2;
                case 27:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 35;
                    else
                        return 3;
                case 28:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 38;
                    else
                        return 6;
                case 35:
                case 39:
                    return 12;
                case 45:
                case 46:
                    return 8;
                case 19:
                case 20:
                case 21:
                case 22:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 37;
                    else
                        return 11;
                case 29:
                case 30:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 35;
                    else
                        return 3;
                case 33:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 38;
                    else
                        return 6;
                case 5:
                case 13:
                case 14:
                case 15:
                case 16:
                    return 22;
                case 18:
                case 25:
                case 41:
                case 42:
                case 43:
                    return 25;
                case 1:
                case 2:
                case 3:
                case 4:
                case 37:
                case 38:
                case 47:
                    return 15;
                case 31:
                case 32:
                case 34:
                case 36:
                case 44:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                    {
                        return 33;
                    }
                    else
                        return 1;
                case 23:
                case 24:
                    return 32;
                case 9:
                case 10:
                case 11:
                case 12:
                case 40:
                    return 18;
                case 6:
                case 7:
                case 8:
                case 17:
                    return 15;
                default:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 33;
                    else
                        return 1;
            }
        }
    }
}