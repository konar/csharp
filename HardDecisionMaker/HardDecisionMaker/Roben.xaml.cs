using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HardDecisionMaker
{
    public partial class Roben : PhoneApplicationPage
    {
        private Accelerometer Acc = null;

        private double _x = 0;
        private double _y = 0;
        private double _z = 0;

        private double _lastp = 0;

        private bool isStarted = false;

        private string[] Choices;
        private double[] Tendencies;
        private int ChoicesTotal = 8;

        private TextBlock[] tbIndexs;
        private int[] PositionIndexes;

        private const double Sensortivity = 0.01;

        private double CurrentSpeed = 0;
        private object CurrentSpeedLocker = new object();
        private bool IsRotating = false;

        private const int ThreshHold = 10;


        public Roben()
        {
            InitializeComponent();
            //InputTestData();
            AdjustLayout();

            ReceiveDataFromMainWindow();
            InitalIndexes();
            Run();
        }

        private void AdjustLayout()
        {
            var width = Application.Current.Host.Content.ActualWidth;
            image1.Height = width * 0.5;
            ContentPanel.Width = width;
            ContentPanel.Height = width;
            transform.Angle = 180.0/ChoicesTotal;

        }

        private void InputTestData()
        {
            Tendencies = new [] { 0.1, 0.1, 0.2, 0.05, 0.05, 0.15, 0.3, 0.05 };
            Choices = new [] { "A", "B", "C", "D", "E", "F", "G", "H"};
            PositionIndexes = new [] {  1,  3, 2,4, 0};
            ChoicesTotal = 5;

            Dispatcher.BeginInvoke(() =>
            {
                PageTitle.Text = "God is considering about it, please wait...";
                PageTitle.Foreground = new SolidColorBrush(Colors.Yellow);
                for(int i=0; i<5; i++)
                {
                    tbIndexs[i].Text = (PositionIndexes[i] + 1).ToString();
                }
            });
        }

        private void ReceiveDataFromMainWindow()
        {
            const string totalPrefix = "HDMaker_Total";
            const string choicePrefix = "HDMaker_Choices";
            const string tendencyPrefix = "HDMaker_Tendencies";

            ChoicesTotal = (int)PhoneApplicationService.Current.State[totalPrefix];
            Choices = (string[])PhoneApplicationService.Current.State[choicePrefix];
            Tendencies = (double[])PhoneApplicationService.Current.State[tendencyPrefix];
            NormalLiseTendencies();
            PositionIndexes = new int[ChoicesTotal];
            for(int i=0; i<ChoicesTotal; i++)
            {
                PositionIndexes[i] = i;
            }
        }

        private void NormalLiseTendencies()
        {
            double sum = Tendencies.Sum(i => i);

            for (int i = 0; i < ChoicesTotal; i++)
            {
                Tendencies[i] /= sum;
            }

        }

        private void Run()
        {
            var thread = new Thread(()=>
                                        {
                                            RandomlizeTable();
                                            EnableAccelerometer();
                                        });

            thread.Start();
        }

        private void RandomlizeTable()
        {
            Dispatcher.BeginInvoke(() =>
                                       {
                                           PageTitle.Text = "Randomizing the state, please wait...";
                                           PageTitle.Foreground = new SolidColorBrush(Colors.White);
                                       });

            var random = new Random(DateTime.Now.Millisecond);

            const int Round = 10;

            for (int t = 0; t < Round; t++ )
            {
                for (int i = 0; i < ChoicesTotal; i++)
                {
                    int j = random.Next(0, ChoicesTotal - 1);

                    Dispatcher.BeginInvoke(() =>
                                               {
                                                   int k = PositionIndexes[j];
                                                   PositionIndexes[j] = PositionIndexes[i];
                                                   PositionIndexes[i] = k;
                                                   tbIndexs[i].Text = (PositionIndexes[i] + 1).ToString();
                                                   tbIndexs[j].Text = (PositionIndexes[j] + 1).ToString();
                                               }
                        );
                    
                    Thread.Sleep(50);
                }
            }

            Dispatcher.BeginInvoke(() =>
            {
                PageTitle.Text = "Please shake your phone as hard as possilbe...";
                PageTitle.Foreground = new SolidColorBrush(Colors.Red);
            });
        }

        private void InitalIndexes()
        {

            var width = Application.Current.Host.Content.ActualWidth;
            var height = Application.Current.Host.Content.ActualHeight;


            tbIndexs = new TextBlock[ChoicesTotal];

            var R = width / 2;

            for (int i = 0; i < ChoicesTotal; i++)
            {
                tbIndexs[i] = new TextBlock
                {
                    Text = (i + 1).ToString(),
                    FontFamily = new FontFamily("Yu Gothic"),
                    FontSize = 40,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Foreground = new SolidColorBrush(Colors.Orange)
                };

                var fontWidth = tbIndexs[i].ActualWidth*2;
                var fontHeight = tbIndexs[i].ActualHeight;

                var left = R * Math.Sin(2 * Math.PI / ChoicesTotal * i) + R - fontWidth / 2;
                var top = R - R * Math.Cos(2 * Math.PI / ChoicesTotal * i) - fontHeight / 2;
                var right = R * 2 - left - fontWidth;
                var bottom = R * 2 - top - fontHeight;

                if (left < 0)
                {
                    right += left;
                    left = 0;
                }

                if (right < 0)
                {
                    left += right;
                    right = 0;
                }

                tbIndexs[i].Margin = new Thickness(left, top, right, bottom);
                ContentPanel.Children.Add(tbIndexs[i]);
            }

        }

        private void Accelerometer_ReadingChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            double x = e.SensorReading.Acceleration.X - _x;
            double y = e.SensorReading.Acceleration.Y - _y;
            double z = e.SensorReading.Acceleration.Z - _z;

            double power = x * x + y * y + z * z;

            if ((Math.Abs(_x) + Math.Abs(_y) +Math.Abs(_z) > 0) && power > Sensortivity)
            {
                Acc.Stop();

                if (power > _lastp)
                {
                    _lastp = power;
                    if(_lastp >= ThreshHold)
                    {
                        isStarted = true;
                        Dispatcher.BeginInvoke(() =>
                                                   {
                                                       PageTitle.Text = "God is considering about it, please wait...";
                                                       PageTitle.Foreground = new SolidColorBrush(Colors.Yellow);
                                                   });
                    }
                }

                if (!isStarted)
                {
                    var newSpeed = power*10;

                    lock (CurrentSpeedLocker)
                    {
                        if (CurrentSpeed < newSpeed)
                        {
                            CurrentSpeed = newSpeed;
                        }

                        if (!IsRotating)
                        {
                            StartRotatingWithSpeed();
                        }
                    }
                }

                Acc.Start();
            }


            _x = e.SensorReading.Acceleration.X;
            _y = e.SensorReading.Acceleration.Y;
            _z = e.SensorReading.Acceleration.Z;

        }

        private void RotateWithConstantSpeedFromTo(double from, double to, double speed)
        {

            var rotateThread = new Thread(()=>
                                            {
                                                for (double i = from; i < to; i += speed)
                                                {
                                                    Thread.Sleep(1);
                                                    Dispatcher.BeginInvoke(() =>
                                                                            {
                                                                                transform.Angle = i;

                                                                            }
                                                                          );
                                                }
                                            }
                                          );
            rotateThread.Start();
            rotateThread.Join();
        }

        private void EnableAccelerometer()
        {
            Acc = new Accelerometer();
            Acc.CurrentValueChanged += Accelerometer_ReadingChanged;
            Acc.Start();
        }

        private void DisableAccelerometer()
        {
            Acc.Stop();
        }

        private void StartRotatingWithSpeed()
        {
            var t = new Thread(() => RotateWithInitalSpeedAndPosition(GetCurrentPosition()));
            t.Start();
        }

        private int GetCurrentPosition()
        {
            double angle = 0;
            Dispatcher.BeginInvoke(() =>
                                       {
                                           angle = transform.Angle;
                                       });
            return Convert.ToInt32(Math.Ceiling(angle*ChoicesTotal/360));
        }

        private void RotateWithInitalSpeedAndPosition(int initalPosition)
        {
            var currentPosition = initalPosition;
            IsRotating = true;

            while(true)
            {
                lock (CurrentSpeedLocker)
                {
                    CurrentSpeed -= (CurrentSpeed * 0.2) * Tendencies[PositionIndexes[currentPosition]];
                    if (CurrentSpeed < 0.04)
                    {
                        IsRotating = false;
                        break;
                    }
                }

                var from = 360*currentPosition/(ChoicesTotal*1.0);
                var to = 360*(currentPosition+1)/(ChoicesTotal*1.0);
                RotateWithConstantSpeedFromTo(from, to, CurrentSpeed);
                currentPosition = (currentPosition + 1) % ChoicesTotal;
            }

            if(isStarted)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    int index = PositionIndexes[currentPosition];
                    PageTitle.Text = string.Format("God's suggestion is {0} : {1}.", index +1, Choices[index]);
                    PageTitle.Foreground = new SolidColorBrush(Colors.Green);
                });

                DisableAccelerometer();
            }
        }

    }
}