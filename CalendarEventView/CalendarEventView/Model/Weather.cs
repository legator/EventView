using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarEventView.Model
{
    public class Weather
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string TimeZone { get; set; }
        public Temperature Temperature { get; set; }

        public Weather()
        {
            Temperature = new Temperature();
        }
    }

    public class Temperature
    {
        public string CurrentValue { get; set; }
        public string SkyCode { get; set; }
        public string SkyText { get; set; }
        public string FeelsLike { get; set; }
        public string Wind { get; set; }
    }
}
