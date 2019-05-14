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

using System.IO;
using PhoneChessEngine;

namespace Gopang
{
    public partial class MainPage : PhoneApplicationPage
    {
        public int StartLeft = 126;
        public int StartTop = 67;
        public int RowDeta = 48;
        public int ColDeta = 48;
        public bool IsPlaying = false;
        private Image[][] chesses = new Image[Constants.Rows][];

        private Position _lastPosition = null;

        private ChessEngine _chessEngine = new ChessEngine();

        private PointState _currentPlayer = PointState.Black;

        private ApplicationBarMenuItem _menuItem1;
        private ApplicationBarMenuItem _menuItem2;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            InitStates();

            AdjustLayout();
            BaseGrid.MouseMove += BaseGrid_MouseMoved;
            BaseGrid.MouseLeftButtonUp += BaseGrid_MouseLeftButtonUp;
            BaseGrid.MouseLeftButtonDown += BaseGrid_MouseLeftButtonDown;
            startBtn.Click += startBtn_Click;
            InitializeAppBar();
        }

        private void InitializeAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            _menuItem1 = new ApplicationBarMenuItem();
            _menuItem1.Text = "Start a new game";

            //GetFiles();

            ApplicationBarIconButton newGamebtn = new ApplicationBarIconButton();
            newGamebtn.IconUri = new Uri("/Images/new.png", UriKind.Relative);
            newGamebtn.Text = "new";
            ApplicationBar.Buttons.Add(newGamebtn);
            newGamebtn.Click += new EventHandler(newBtn_Click);

            ApplicationBarIconButton undoBtn = new ApplicationBarIconButton();
            undoBtn.IconUri = new Uri("/Images/back.png", UriKind.Relative);
            undoBtn.Text = "back";
            ApplicationBar.Buttons.Add(undoBtn);
            undoBtn.Click += new EventHandler(undoBtn_Click);

            ApplicationBarIconButton saveBtn = new ApplicationBarIconButton();
            saveBtn.IconUri = new Uri("/Images/save.png", UriKind.Relative);
            saveBtn.Text = "save";
            ApplicationBar.Buttons.Add(saveBtn);
            saveBtn.Click += new EventHandler(saveBtn_Click);

            ApplicationBarIconButton shareBtn = new ApplicationBarIconButton();
            shareBtn.IconUri = new Uri("/Images/share.png", UriKind.Relative);
            shareBtn.Text = "share";
            ApplicationBar.Buttons.Add(shareBtn);
            shareBtn.Click += new EventHandler(shareBtn_Click);

            //undoBtn.IsEnabled = false;

            _menuItem1.Click += new EventHandler(menuItem1_Click);
            ApplicationBar.MenuItems.Add(_menuItem1);

        }

        private void shareBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void TestEngine()
        {
            //_chessEngine.Take(new Position(6, 3));
            //_chessEngine.Take(new Position(6, 4));
            //_chessEngine.Take(new Position(7, 2));
            //_chessEngine.Take(new Position(8, 3));
            //_chessEngine.Take(new Position(7, 5));
            //_chessEngine.Take(new Position(8, 5));
            //_chessEngine.Take(new Position(9, 5));

            //_chessEngine.Place(new Position(5, 3));
            //_chessEngine.Place(new Position(5, 4));
            //_chessEngine.Place(new Position(6, 2));
            //_chessEngine.Place(new Position(7, 3));
            //_chessEngine.Place(new Position(8, 4));
            //_chessEngine.Place(new Position(7, 4));
            //_chessEngine.Place(new Position(9, 4));

            //_chessEngine.Place();
        }

        private void ClearOnePosition(Position position)
        {
            if (position != null)
            {
                chesses[position.X][position.Y].Visibility = Visibility.Collapsed;
            }
        }

        private void undoBtn_Click(object sender, EventArgs e)
        {
            ClearOnePosition(_chessEngine.StepbackC());
            ClearOnePosition(_chessEngine.StepbackH());

            Position lastPosition = _chessEngine.LastC;
            if (lastPosition != null)
            {
                SetLastStepIndicator(lastPosition);
            }

            playResultIndicator.Text = "";
            playResultIndicator.Visibility = Visibility.Collapsed;

        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            InitForRestart();
            IsPlaying = true;
            startBtn.Content = "Restart";
        }
        private void menuItem2_Click(object sender, EventArgs e)
        {
            ApplicationBar.MenuItems.Remove(_menuItem1);
            MessageBox.Show("Button clicked");

        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            _menuItem2 = new ApplicationBarMenuItem();
            _menuItem2.Text = "Undo";

            _menuItem2.Click += new EventHandler(menuItem2_Click);
            ApplicationBar.MenuItems.Add(_menuItem2);
            MessageBox.Show("Started");
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.IsVisible = false;
        }

        void startBtn_Click(object sender, RoutedEventArgs e)
        {
            InitForRestart();
            IsPlaying = true;
            startBtn.Content = "Restart";
        }

        void BaseGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsPlaying)
            {
                return;
            }

            var pointerPosition = GetPosition(e.GetPosition(BaseGrid));


            if (IsValidCursorPosition(pointerPosition) && pointerPosition.Equals(_lastPosition))
            {
                if (_chessEngine[_lastPosition] == PointState.Border)
                {
                    HumanPlaceOneChess(pointerPosition);
                    if (_chessEngine.IsGameOver(pointerPosition))
                    {
                        playResultIndicator.Visibility = Visibility.Visible;
                        ShowResult("You Won, Congratulations! Please click Restart button.");
                        IsPlaying = false;
                        return;
                    }

                    if (_chessEngine.StepCount() > Constants.Rows * Constants.Cols / 2)
                    {
                        playResultIndicator.Visibility = Visibility.Visible;
                        ShowResult("Draw, Congratulations! Please click Restart button.");
                        IsPlaying = false;
                        return;
                    }

                    var bestPosition = _chessEngine.Place();
                    ComputerPlaceOneChess(bestPosition);

                    if (_chessEngine.IsGameOver(bestPosition))
                    {
                        playResultIndicator.Visibility = Visibility.Visible;
                        playResultIndicator.Text = "";
                        ShowResult("You Lost, Come on next time! Please click Restart button.");
                        IsPlaying = false;
                        return;

                    }
                }
            }
        }

        void BaseGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsPlaying)
            {
                return;
            }

            var pointerPosition = GetPosition(e.GetPosition(BaseGrid));
            if (IsValidCursorPosition(pointerPosition))
            {
                if (IsValidCursorPosition(_lastPosition))
                {
                    if (_chessEngine[_lastPosition] == PointState.Border || _chessEngine[_lastPosition] == PointState.None)
                    {
                        chesses[_lastPosition.X][_lastPosition.Y].Visibility = Visibility.Collapsed;
                        _chessEngine[_lastPosition] = PointState.None;
                    }
                }

                _lastPosition = pointerPosition;

                if (_chessEngine[_lastPosition] == PointState.None)
                {
                    _chessEngine[_lastPosition] = PointState.Border;
                    chesses[_lastPosition.X][_lastPosition.Y].Source = (_currentPlayer == PointState.Black) ? WhiteCandiaterPic.Source : BlackCandiatePic.Source;
                    chesses[_lastPosition.X][_lastPosition.Y].Visibility = Visibility.Visible;
                    UpdateStatus(PlayingState.ConfirmChoice, "Playing. Please click this chess again to confirm your choice");
                }

            }
        }

        void BaseGrid_MouseMoved(object sender, MouseEventArgs e)
        {
            if (!IsPlaying)
            {
                return;
            }

            var pointerPosition = GetPosition(e.GetPosition(BaseGrid));

            if (pointerPosition.Equals(_lastPosition))
            {
                return;
            }

            if (IsValidCursorPosition(_lastPosition))
            {
                if (_chessEngine[_lastPosition] == PointState.Border || _chessEngine[_lastPosition] == PointState.None)
                {
                    chesses[_lastPosition.X][_lastPosition.Y].Visibility = Visibility.Collapsed;
                    _chessEngine[_lastPosition] = PointState.None;
                }
            }

            if (IsValidCursorPosition(pointerPosition))
            {
                _lastPosition = pointerPosition;

                if (_chessEngine[_lastPosition] == PointState.None)
                {
                    chesses[_lastPosition.X][_lastPosition.Y].Source = (_currentPlayer == PointState.Black) ? WhiteBorderPic.Source : BlackBorderPic.Source;
                    chesses[_lastPosition.X][_lastPosition.Y].Visibility = Visibility.Visible;
                }

                UpdateStatus(PlayingState.SelectingCandiate, "Playing. Please click your mouse to mark it as an candidate.");
            }


        }

        private void InitStates()
        {
            _chessEngine.Initialize(_currentPlayer);
        }

        private void AdjustButtonLocation()
        {
            var width = Application.Current.Host.Content.ActualWidth;
            var height = Application.Current.Host.Content.ActualHeight;
            int oHeight = 68;
            int oLeft = 180;
            int oTop = -12;
            int oRight = 164;

            startBtn.Height = oHeight * height / 800;
            startBtn.Margin = new Thickness(oLeft * width / 480, oTop * height / 800, oRight * width / 480, 0);
        }

        private void ShowResult(string msg)
        {
            playResultIndicator.Text = msg;
            var height = Application.Current.Host.Content.ActualHeight;
            var pHeight = playResultIndicator.ActualHeight;
            playResultIndicator.Margin = new Thickness(0, height/2, 0, height/2-pHeight);
        }

        private void AdjustLayout()
        {
            AdjustButtonLocation();
            InitAllChesses();
            ShowResult("You Won. Please click Restart button.");
        }

        private void InitAllChesses()
        {
            var width = Application.Current.Host.Content.ActualWidth;
            var height = Application.Current.Host.Content.ActualHeight;
            StartLeft = Convert.ToInt32(56 * width / 480);
            StartTop = Convert.ToInt32(86 * height / 800); ;

            RowDeta = Convert.ToInt32(48 * width / 480);
            ColDeta = Convert.ToInt32(48 * width / 480);

            for (int i = 0; i < Constants.Rows; i++)
            {
                chesses[i] = new Image[Constants.Cols];
                for (int j = 0; j < Constants.Cols; j++)
                {
                    var image = new Image();
                    image.Height = RowDeta;
                    image.Width = ColDeta;
                    image.Source = WhiteChessPic.Source;
                    var left = StartLeft + j * ColDeta - 0.5 * ColDeta;
                    var top = StartTop + i * RowDeta - 0.5 * RowDeta;
                    var right = width - left - ColDeta;
                    var bottom = height - top - RowDeta;
                    image.Margin = new Thickness(left, top, right, bottom);
                    image.Visibility = Visibility.Collapsed;
                    BaseGrid.Children.Add(image);
                    chesses[i][j] = image;
                }
            }

            Canvas.SetZIndex(LastStepIndicator, 100);
            Canvas.SetZIndex(playResultIndicator, 101);
        }

        private void InitForRestart()
        {
            LastStepIndicator.Visibility = Visibility.Collapsed;
            playResultIndicator.Visibility = Visibility.Collapsed;

            for (int i = 0; i < Constants.Rows; i++)
            {
                for (int j = 0; j < Constants.Cols; j++)
                {
                    chesses[i][j].Visibility = Visibility.Collapsed;
                }
            }

            _currentPlayer = (_currentPlayer == PointState.White) ? PointState.Black : PointState.White;

            _chessEngine.Initialize(_currentPlayer);

            TestEngine();

            if (_currentPlayer == PointState.Black)
            {
                ComputerPlaceOneChess(_chessEngine.Place());
            }

            UpdateStatus(PlayingState.SelectingCandiate, "Playing. Please select an candidate of your next move.");

        }

        private void PlaceOneChess(Position p, PointState state)
        {
            if (state != PointState.Black && state != PointState.White)
            {
                return;
            }

            chesses[p.X][p.Y].Source = (state == PointState.Black) ? BlackChessPic.Source : WhiteChessPic.Source;
            chesses[p.X][p.Y].Visibility = Visibility.Visible;
        }

        private void ComputerPlaceOneChess(Position p)
        {
            PlaceOneChess(p, _currentPlayer);
            SetLastStepIndicator(p);
        }

        private void HumanPlaceOneChess(Position p)
        {
            _chessEngine.Take(p);
            var state = (_currentPlayer == PointState.Black) ? PointState.White : PointState.Black;
            PlaceOneChess(p, state);

            //tbStatus.Text = "Playing. Please select an candidate of your next move.";
        }

        private bool IsValidCursorPosition(Position p)
        {
            return p != null && p.X > -1 && p.X < Constants.Rows && p.Y > -1 && p.Y < Constants.Cols;
        }

        private void SetLastStepIndicator(Position p)
        {
            LastStepIndicator.Margin = chesses[p.X][p.Y].Margin;
            LastStepIndicator.Visibility = Visibility.Visible;
        }

        private void UpdateStatus(PlayingState state, string msg)
        {
            //switch (state)
            //{
            //    case PlayingState.Ready:
            //        tbStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            //        break;

            //    case PlayingState.SelectingCandiate:
            //        tbStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 138, 43, 226));
            //        break;

            //    case PlayingState.ConfirmChoice:
            //        tbStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            //        break;

            //    case PlayingState.OverWon:
            //        tbStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0));
            //        break;

            //    case PlayingState.OverLost:
            //        tbStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            //        break;

            //    default:
            //        break;
            //}

            //tbStatus.Text = msg;
        }

        private Position GetPosition(Point p)
        {
            int x = -1;
            int y = -1;

            for (int i = 0; i <= Constants.Rows; i++)
            {
                if (StartTop - 0.5 * RowDeta + i * (RowDeta) < p.Y && p.Y <= StartTop + 0.5 * RowDeta + i * (RowDeta))
                {
                    x = i;
                    break;
                }
            }

            for (int j = 0; j <= Constants.Cols; j++)
            {
                if (StartLeft - 0.5 * ColDeta + j * ColDeta < p.X && p.X <= StartLeft + 0.5 * ColDeta + j * ColDeta)
                {
                    y = j;
                    break;
                }
            }

            return new Position(x, y);

        }
   }
}