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
    public partial class Calculate : PhoneApplicationPage
    {
        private bool isPreciseToTime = false;
        public Calculate()
        {
            InitializeComponent();
            CalculateFrom(GetStartDatetime());
        }

        public void CalculateFrom(DateTime startDate)
        {
            PageTitle.Text = string.Format("Since {0}",  startDate.ToString(isPreciseToTime ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd"));

            DateTime now = DateTime.Now;
            TimeSpan timeSpan = now-startDate;
            int yearsEls = GetYearsBetween(startDate, now);
            int monthsEls = GetMonthsBetween(startDate, now);
            int daysEls = Convert.ToInt32(Math.Floor(timeSpan.TotalDays));


            int monthsREls = GetMonthsBetween(startDate.AddYears(yearsEls), now);
            int daysREls = Convert.ToInt32(Math.Floor((now - startDate.AddMonths(monthsEls)).TotalDays));

            string total = string.Empty;
            total = yearsEls > 0 ? string.Format("{0} year{1}", yearsEls, yearsEls > 1 ? "s" : string.Empty) : total;
            total = monthsREls > 0 ? string.Format("{0} {1} month{2}", total, monthsREls, monthsREls > 1 ? "s" : string.Empty) : total;
            total = daysREls > 0 ? string.Format("{0} {1} day{2}", total, daysREls, daysREls > 1 ? "s" : string.Empty) : total;

            if (isPreciseToTime)
            {
                var hoursREls = Convert.ToInt32(Math.Floor((now - startDate.AddDays(daysEls)).TotalHours));
                var hourEls =Convert.ToInt32(Math.Floor(timeSpan.TotalHours));
                var minuteEls = Convert.ToInt32(Math.Floor(timeSpan.TotalMinutes));
                var minuteREls = Convert.ToInt32(Math.Floor((now - startDate.AddHours(hourEls)).TotalMinutes));

                total = hoursREls > 0 ? string.Format("{0} {1} hour{2}", total, hoursREls, hoursREls > 1 ? "s" : string.Empty) : total;

                total = minuteREls > 0 ? string.Format("{0} {1} minute{2}", total, minuteREls, minuteREls > 1 ? "s" : string.Empty) : total;

                ElpHours.Text = string.Format("The {0} hour", GetNumberOrderString(hourEls + 1));
                ElpMinutes.Text = string.Format("The {0} minute", GetNumberOrderString(minuteEls + 1));
            }

            ElpTotal.Text = total;

            ElpYears.Text = string.Format("The {0} year", GetNumberOrderString(yearsEls + 1));
            ElpMonths.Text = string.Format("The {0} month", GetNumberOrderString(monthsEls + 1));
            ElpWeeks.Text = string.Format("The {0} week", GetNumberOrderString(daysEls / 7 + 1));
            ElpDays.Text = string.Format("The {0} day", GetNumberOrderString(daysEls + 1));

        }

        public int GetMonthsBetween(DateTime startDate, DateTime endDate)
        {
            int gapYear = GetYearsBetween(startDate, endDate);
            int sMonth = endDate.Month - startDate.Month;
            sMonth = sMonth < 0 ? sMonth + 12 : sMonth;
            int gapMonths = gapYear*12 + sMonth;

            if(startDate.AddMonths(gapMonths) > endDate)
            {
                gapMonths--;
            }

            return gapMonths;
        }

        public int GetYearsBetween(DateTime startDate, DateTime endDate)
        {
            int yearBetween = endDate.Year - startDate.Year;
            if (startDate.AddYears(yearBetween) > endDate)
            {
                yearBetween--;
            }

            return yearBetween;
        }

        public DateTime GetStartDatetime()
        {
            var startDate = (DateTime?)PhoneApplicationService.Current.State["MemoriableDate"];
            var startTime = (DateTime?)PhoneApplicationService.Current.State["MemoriableTime"];
            if (startDate == null)
            {
                throw new Exception("Invalid switch");
            }

            DateTime dt;

            if (startTime == null)
            {
                dt = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                isPreciseToTime = false;
            }
            else
            {
                dt = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, startTime.Value.Hour, startTime.Value.Minute, 0);
                isPreciseToTime = true;
            }

            return dt;
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