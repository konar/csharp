using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SpeicalDays
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PrepareDataBeforePageSwitch())
            {
                return;
            }

            if (ActionPicker.SelectedValue.ToString().IndexOf("now") > 0)
            {
                NavigationService.Navigate(new Uri("/Calculate.xaml", UriKind.Relative));
            }
            else if (ActionPicker.SelectedValue.ToString().IndexOf("soon") > 0)
            {
                NavigationService.Navigate(new Uri("/Forecast.xaml", UriKind.Relative));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private bool PrepareDataBeforePageSwitch()
        {
            if(RDPicker.Value == null)
            {
                MessageBox.Show("Please set the date first!");
                return false;
            }

            PhoneApplicationService.Current.State["MemoriableDate"] = RDPicker.Value;
            PhoneApplicationService.Current.State["MemoriableTime"] = RTPicker.Value;

            return true;
        }


    }
}