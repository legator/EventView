using CalendarEventView.ViewModel;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CalendarEventView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainViewModel ViewModel { get; private set; }

        public App()
        {
            DispatcherHelper.Initialize();
            ViewModel = new MainViewModel();
        }
    }
}
