using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalendarEventView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            App.ViewModel.Log();
            Timers();
        }

        private void Timers()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(TimerChange_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
            InitData();

            System.Windows.Threading.DispatcherTimer weatherTimer = new System.Windows.Threading.DispatcherTimer();
            weatherTimer.Tick += new EventHandler(WeatherTimerChange_Tick);
            weatherTimer.Interval = new TimeSpan(0, 30, 0);
            weatherTimer.Start();
            GetWeather();
        }

        #region windows control
        private void WindowMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y < 38)
            {
                DragMove();
            }
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void ConnectedApp_Click(object sender, RoutedEventArgs e)
        {
            //App.ViewModel.Connected = !App.ViewModel.Connected;
            App.ViewModel.Log();
        }

        private void WeatherTimerChange_Tick(object sender, EventArgs e)
        {
            App.ViewModel.GetWeather();
        }

        private void GetWeather()
        {
            App.ViewModel.GetWeather();
        }

        private void TimerChange_Tick(object sender, EventArgs e)
        {
            InitData();
        }

        private void InitData()
        {
            DateTime dt = new DateTime();
            dt = DateTime.Now;

            Year.Text = dt.Year.ToString();
            Month.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month);
            Day.Text = dt.Day.ToString();
            DayName.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(dt.DayOfWeek);
        }
    }
}
