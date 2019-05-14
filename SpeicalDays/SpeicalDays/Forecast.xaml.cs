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
    public partial class Forecast : PhoneApplicationPage
    {

        public const int MAX_CAN = 7;
        public TextBlock[] tbCandidates;
        public Forecast()
        {
            InitializeComponent();
            FullfillTopNSpecialDays();
        }

        public void FullfillTopNSpecialDays()
        {
            var startDate = GetStartDatetime();

            PageTitle.Text = string.Format("For {0}", startDate.ToString("yyyy-MM-dd"));

            var candiatesList = GetTopNSpecialDays(startDate);
            tbCandidates  = new TextBlock[MAX_CAN];
            for(int i=0; i<MAX_CAN; i++)
            {
                tbCandidates[i] = new TextBlock()
                                      {
                                          Text = string.Format("You will hit {0} on {1}", candiatesList[i].Event, candiatesList[i].Date.ToString("yyyy-MM-dd")),
                                          FontSize = 30,
                                          Foreground = new SolidColorBrush(Colors.Orange),
                                          TextWrapping = TextWrapping.Wrap
                                      };
                Grid.SetRow(tbCandidates[i], i);
                ContentPanel.Children.Add(tbCandidates[i]);
            }

        }

        public List<DateEvent> GetTopNSpecialDays(DateTime startDate)
        {
            var currentDate = DateTime.Now;
            var candiatesList = new List<DateEvent>();
            while(candiatesList.Count <= MAX_CAN)
            {
                var timeSpan = currentDate - startDate;
                int daysEls = Convert.ToInt32(Math.Floor(timeSpan.TotalDays)) + 1;
                if(IsSameDigitals(daysEls) || IsDigitalWithZero(daysEls))
                {
                    candiatesList.Add(new DateEvent()
                                          {
                                              Date = currentDate,
                                              Event = string.Format( "the {0} day", GetNumberOrderString(daysEls))
                                          });
                }

                currentDate = currentDate.AddDays(1);
            }

            return candiatesList;
        }

        public DateTime GetStartDatetime()
        {
            var startDate = (DateTime?)PhoneApplicationService.Current.State["MemoriableDate"];
            if (startDate == null)
            {
                throw new Exception("Invalid switch");
            }

            return new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
        }

        public bool IsSameDigitals(Int32 number)
        {
            var digitals = new List<int>();
            while(number != 0)
            {
                int n = number%10;
                digitals.Add(n);

                number /= 10;
            }

            int i = digitals.First();

            return digitals.All(j => j == i);

        }

        private bool IsDigitalWithZero(Int32 number)
        {
            return number%10 == 0;
        }

        public string GetNumberOrderString(int number)
        {
            string result;

            switch (number)
            {
                case 1:
                    result = "1st";
                    break;
                case 2:
                    result = "2nd";
                    break;
                case 3:
                    result = "3rd";
                    break;
                default:
                    result = string.Format("{0}th", number);
                    break;
            }

            return result;
        }
    }
}