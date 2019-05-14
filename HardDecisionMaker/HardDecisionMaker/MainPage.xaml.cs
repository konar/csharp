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
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;


namespace HardDecisionMaker
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor


        private int ChoicesTotal = 2;

        private const int MaxChoicesNumber = 9;

        private TextBlock[] TbIndexs;
        private TextBox[] TbChoices;
        private TextBox[] TbTendencies;
        private Border[][] Borders;

        public MainPage()
        {
            InitializeComponent();
            InitalAllTheChoices();
            InitalFirstChoice();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var header = ((MenuItem)sender).Header.ToString();
            int index = header.ElementAt(7) - '1';

            DeleteChoice(index);
        }

        private void InitalFirstChoice()
        {
            ShowOneChoice(0);
            ShowOneChoice(1);
        }

        private void DeleteChoice(int index)
        {
            for(int i=index; i<ChoicesTotal-1; i++)
            {
                TbChoices[i].Text = TbChoices[i + 1].Text;
                TbTendencies[i].Text = TbTendencies[i + 1].Text;
            }

            if (CheckWhetherNeedAutoUpdateTendencies(ChoicesTotal))
            {
                AutoUpdateTendencies(ChoicesTotal - 2);
            }

            ChoicesTotal--;

            TbChoices[ChoicesTotal].Visibility = Visibility.Collapsed;
            TbTendencies[ChoicesTotal].Visibility = Visibility.Collapsed;

            for (int i = 0; i < 3; i++)
            {
                Borders[ChoicesTotal][i].Visibility = Visibility.Collapsed;
            }

            TbChoices[ChoicesTotal].Text = "";
            TbTendencies[ChoicesTotal].Text = "";

            Grid.SetRow(ButtonGrid, ChoicesTotal+1);
        }

        private bool CheckWhetherNeedAutoUpdateTendencies(int index)
        {
            if (index < 1)
            {
                return true;
            }

            for (int i = 0; i < index; i++)
            {
                double tendency;
                if (!double.TryParse(TbTendencies[i].Text, out tendency))
                {
                    return false;
                }

                const double epsilon = 0.02;
                if (Math.Abs(tendency - 1.0 / index) > epsilon)
                {
                    return false;
                }
            }

            return true;
        }

        private void AutoUpdateTendencies(int index)
        {
            for (int i = 0; i <= index; i++)
            {
                TbTendencies[i].Text = string.Format("{0:0.00}", 1.0 / (index + 1));
            }
        }

        private void CheckOrAutoUpdateTendencies(int index)
        {
            if (CheckWhetherNeedAutoUpdateTendencies(index))
            {
                AutoUpdateTendencies(index);
            }
        }

        private void ShowOneChoice(int index)
        {
            for (int i = 0; i < 3; i++)
            {
                Borders[index][i].Visibility = Visibility.Visible;
            }

            TbChoices[index].Visibility = Visibility.Visible;
            TbTendencies[index].Visibility = Visibility.Visible;

            CheckOrAutoUpdateTendencies(index);

            Grid.SetRow(ButtonGrid, index + 2);
        }

        private void InitalAllTheChoices()
        {
            TbIndexs = new TextBlock[MaxChoicesNumber];
            TbChoices = new TextBox[MaxChoicesNumber];
            TbTendencies = new TextBox[MaxChoicesNumber];
            Borders = new Border[MaxChoicesNumber][];

            var width = Application.Current.Host.Content.ActualWidth;
            var height = Application.Current.Host.Content.ActualHeight;

            const int referenceObjectWidth = 480;
            const int referenceObjectHeight = 800;

            for (int i = 0; i < MaxChoicesNumber; i++)
            {
                Borders[i] = new Border[3];

                //for indexes
                Borders[i][0] = new Border
                                   {
                                       BorderBrush = GetColorFromHexa("#FF00EC00"),
                                       BorderThickness = new Thickness(2),
                                       Visibility = Visibility.Collapsed
                                   };
                Grid.SetRow(Borders[i][0], i + 1);
                Grid.SetColumn(Borders[i][0], 0);

                TbIndexs[i] = new TextBlock
                                  {
                                      VerticalAlignment = VerticalAlignment.Center,
                                      Text = string.Format("{0}.", i + 1),

                                  };


                Borders[i][0].Child = TbIndexs[i];
                ContentPanel.Children.Add(Borders[i][0]);

                //for choices

                Borders[i][1] = new Border
                                  {
                                      BorderBrush = GetColorFromHexa("#FF00EC00"),
                                      BorderThickness = new Thickness(2),
                                      Visibility = Visibility.Collapsed
                                  };

                Grid.SetRow(Borders[i][1], i + 1);
                Grid.SetColumn(Borders[i][1], 1);

                TbChoices[i] = new TextBox
                                   {
                                       VerticalAlignment = VerticalAlignment.Top,
                                       Height = 75 * height / referenceObjectHeight,
                                       Margin = new Thickness(40 * width / referenceObjectWidth, 44 * height / referenceObjectHeight, 78 * width / referenceObjectWidth, 0),
                                       Visibility = Visibility.Collapsed,
                                       InputScope = new InputScope
                                       {
                                           Names = { new InputScopeName()
                                                                                 {
                                                                                     NameValue = InputScopeNameValue.Text
                                                                                 }
                                                                        }
                                       }
                                   };

                Grid.SetRow(TbChoices[i], i);
                Grid.SetRowSpan(TbChoices[i], 3);
                Grid.SetColumn(TbChoices[i], 0);
                Grid.SetColumnSpan(TbChoices[i], 3);

                ContentPanel.Children.Add(Borders[i][1]);
                ContentPanel.Children.Add(TbChoices[i]);


                //for tendencies
                Borders[i][2] = new Border
                              {
                                  BorderBrush = GetColorFromHexa("#FF00EC00"),
                                  BorderThickness = new Thickness(2),
                                  Visibility = Visibility.Collapsed
                              };

                Grid.SetRow(Borders[i][2], i + 1);
                Grid.SetColumn(Borders[i][2], 2);


                TbTendencies[i] = new TextBox
                                      {
                                          VerticalAlignment = VerticalAlignment.Top,
                                          Height = 75 * height / referenceObjectHeight,
                                          Margin = new Thickness(298 * width / referenceObjectWidth, 44 * height / referenceObjectHeight, 0, 0),
                                          Visibility = Visibility.Collapsed,
                                          InputScope = new InputScope
                                                           {
                                                               Names = { new InputScopeName()
                                                                                 {
                                                                                     NameValue = InputScopeNameValue.Number
                                                                                 }
                                                                        }
                                                           }
                                      };

                Grid.SetRow(TbTendencies[i], i);
                Grid.SetRowSpan(TbTendencies[i], 3);
                Grid.SetColumn(TbTendencies[i], 1);
                Grid.SetColumnSpan(TbTendencies[i], 3);

                ContentPanel.Children.Add(Borders[i][2]);
                ContentPanel.Children.Add(TbTendencies[i]);

                var menuItem = new MenuItem
                {
                    Header = string.Format("Delete {0}", i + 1)
                };
                menuItem.Click += MenuItem_Click;

                ContextMenu cm = new ContextMenu
                {
                    Background = new SolidColorBrush(Colors.White),
                    ItemsSource = new List<MenuItem>() { menuItem }
                };

                ContextMenuService.SetContextMenu(TbIndexs[i], cm);
            }
        }

        private void BtnAddNewChoice_Click(object sender, RoutedEventArgs e)
        {
            if (ChoicesTotal == MaxChoicesNumber)
            {
                MessageBox.Show("You can not add more choices, ^-^");
                return;
            }

            ChoicesTotal++;

            ShowOneChoice(ChoicesTotal - 1);

            //Grid.SetRow(BtnAddNewChoice, ChoicesTotal + 1);
        }

        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (PrepareDataBeforePageSwitch())
            {
                NavigationService.Navigate(new Uri("/Roben.xaml", UriKind.Relative));
            }
        }

        private bool PrepareDataBeforePageSwitch()
        {
            const string totalPrefix = "HDMaker_Total";
            const string choicePrefix = "HDMaker_Choices";
            const string tendencyPrefix = "HDMaker_Tendencies";
            if (ValidateInput())
            {
                PhoneApplicationService.Current.State[totalPrefix] = ChoicesTotal;
                PhoneApplicationService.Current.State[choicePrefix] = TbChoices.Take(ChoicesTotal).Select(choice => choice.Text).ToArray();
                PhoneApplicationService.Current.State[tendencyPrefix] = TbTendencies.Take(ChoicesTotal).Select(tendency => double.Parse(tendency.Text)).ToArray();
                return true;
            }

            return false;
        }

        private bool ValidateInput()
        {
            for (int i = 1; i <= ChoicesTotal; i++)
            {
                if (string.IsNullOrWhiteSpace(TbChoices[i - 1].Text))
                {
                    MessageBox.Show(string.Format("Choice {0} can not be empty!", i));
                    return false;
                }

                double tendency;

                if (!double.TryParse(TbTendencies[i - 1].Text, out tendency) || tendency <= 0)
                {
                    MessageBox.Show(string.Format("Tendency {0} is invalid, it should be a value bigger than zero!", i));

                    return false;
                }

            }

            return true;
        }
    }
}