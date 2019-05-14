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

namespace Gopang
{
    public partial class MainPage : PhoneApplicationPage
    {
        public const int Rows = 15;
        public const int Cols = 9;
        public int StartLeft = 126;
        public int StartTop = 67;
        public int RowDeta = 48;
        public int ColDeta = 48;
        public bool IsPlaying = false;
        private Image[][] chesses = new Image[Rows][];

        private int _curX = -1;
        private int _curY = -1;



        private PointState[][] globalState = new PointState[Rows][];

        private List<Postion> humanHistory = new List<Postion>() { };
        private List<Postion> computerHistory = new List<Postion>() { };
        private int currentCursor = -1;

        private PlayerRole _currentPlayer = PlayerRole.White;

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

            var pointerPostion = GetPostion(e.GetPosition(BaseGrid));
            if (IsValidCursorPostion(pointerPostion))
            {
                if (globalState[pointerPostion.X][pointerPostion.Y] == PointState.Border)
                {
                    _curX = pointerPostion.X;
                    _curY = pointerPostion.Y;

                    HumanPlaceOneChess(pointerPostion);
                    if (IsOver(pointerPostion))
                    {
                        playResultIndicator.Visibility = Visibility.Visible;
                        ShowResult("You Won, Congratulations! Please click Restart button.");
                        IsPlaying = false;
                        return;
                    }

                    if (currentCursor == 0)
                    {
                        startBtn.Content = "Restart";
                    }

                    if (currentCursor > Rows * Cols / 2)
                    {
                        playResultIndicator.Visibility = Visibility.Visible;
                        ShowResult("Draw, Congratulations! Please click Restart button.");
                        IsPlaying = false;
                        return;
                    }

                    int bestx = -1, besty = -1; int m;
                    if (currentCursor < 1)
                    {
                        Random random = new Random();
                        while (true)
                        {

                            m = random.Next(0, 7);
                            switch (m)
                            {
                                case 0: bestx = _curX - 1; besty = _curY - 1; break;
                                case 1: bestx = _curX; besty = _curY - 1; break;
                                case 2: bestx = _curX + 1; besty = _curY + 1; break;
                                case 3: bestx = _curX - 1; besty = _curY; break;
                                case 4: bestx = _curX - 1; besty = _curY + 1; break;
                                case 5: bestx = _curX; besty = _curY + 1; break;
                                case 6: bestx = _curX + 1; besty = _curY; break;
                                case 7: bestx = _curX + 1; besty = _curY + 1; break;
                            }
                            if (IsValidCursorPostion(new Postion(bestx, besty)) && globalState[bestx][besty] == PointState.None) break;
                        }
                    }
                    else
                    {
                        bool suc = true;
                        for (m = 1; m <= currentCursor; m++)
                        {
                            if (if5(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty)) { suc = false; break; }
                            if (if5(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty)) { suc = false; break; }
                        }



                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (if4(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty) == 0) { suc = false; break; }
                                if (if4(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty) == 0) { suc = false; break; }
                            }

                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (if3(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty) == 0) { suc = false; break; }
                                if (if3(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty) == 0) { suc = false; break; }
                            }
                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (if4(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty) == 1) { suc = false; break; }
                                if (if4(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty) == 1) { suc = false; break; }
                            }
                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (if3(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty) == 1) { suc = false; break; }
                                if (if3(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty) == 1) { suc = false; break; }
                            }
                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (if4(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty) == 2) { suc = false; break; }
                                if (if4(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty) == 2) { suc = false; break; }
                            }

                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (if3(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty) == 2) { suc = false; break; }
                                if (if3(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty) == 2) { suc = false; break; }
                            }
                        if (suc)
                            for (m = 1; m <= currentCursor; m++)
                            {
                                if (ifer(computerHistory[currentCursor - m].X, computerHistory[currentCursor - m].Y, ref bestx, ref besty)) { suc = false; break; }
                                if (ifer(humanHistory[currentCursor - m + 1].X, humanHistory[currentCursor - m + 1].Y, ref bestx, ref besty)) { suc = false; break; }
                            }

                        if (m > currentCursor)
                        {
                            playResultIndicator.Visibility = Visibility.Visible;
                            ShowResult("Here, Come on next time!");
                            IsPlaying = false;
                            return;
                        }
                    }

                    var bestPostion = new Postion(bestx, besty);
                    ComputePlaceOneChess(bestPostion);

                    if (IsOver(bestPostion))
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

            var pointerPostion = GetPostion(e.GetPosition(BaseGrid));
            if (IsValidCursorPostion(pointerPostion))
            {
                if (IsValidCursorPostion(new Postion(_curX, _curY)))
                {
                    if (globalState[_curX][_curY] == PointState.Border || globalState[_curX][_curY] == PointState.None)
                    {
                        chesses[_curX][_curY].Visibility = Visibility.Collapsed;
                        globalState[_curX][_curY] = PointState.None;
                    }
                }

                _curX = pointerPostion.X;
                _curY = pointerPostion.Y;

                if (globalState[_curX][_curY] == PointState.None)
                {
                    globalState[_curX][_curY] = PointState.Border;
                    chesses[_curX][_curY].Source = (_currentPlayer == PlayerRole.White) ? WhiteCandiaterPic.Source : BlackCandiatePic.Source;
                    chesses[_curX][_curY].Visibility = Visibility.Visible;
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

            var pointerPostion = GetPostion(e.GetPosition(BaseGrid));

            if (pointerPostion.X == _curX && pointerPostion.Y == _curY)
            {
                return;
            }

            if (IsValidCursorPostion(new Postion(_curX, _curY)))
            {
                if (globalState[_curX][_curY] == PointState.Border || globalState[_curX][_curY] == PointState.None)
                {
                    chesses[_curX][_curY].Visibility = Visibility.Collapsed;
                    globalState[_curX][_curY] = PointState.None;
                }
            }

            _curX = -1;
            _curY = -1;

            if (IsValidCursorPostion(pointerPostion))
            {
                _curX = pointerPostion.X;
                _curY = pointerPostion.Y;

                if (globalState[_curX][_curY] == PointState.None)
                {
                    chesses[_curX][_curY].Source = (_currentPlayer == PlayerRole.White) ? WhiteBorderPic.Source : BlackBorderPic.Source;
                    chesses[_curX][_curY].Visibility = Visibility.Visible;
                }

                UpdateStatus(PlayingState.SelectingCandiate, "Playing. Please click your mouse to mark it as an candidate.");
            }


        }

        private void InitStates()
        {
            for (int i = 0; i < Rows; i++)
            {
                globalState[i] = new PointState[Cols];

                for (int j = 0; j < Cols; j++)
                {
                    globalState[i][j] = PointState.None;
                }
            }
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

            for (int i = 0; i < Rows; i++)
            {
                chesses[i] = new Image[Cols];
                for (int j = 0; j < Cols; j++)
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

            humanHistory = new List<Postion>() { };
            computerHistory = new List<Postion>() { };
            currentCursor = -1;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    chesses[i][j].Visibility = Visibility.Collapsed;
                    globalState[i][j] = PointState.None;
                }
            }

            _currentPlayer = (_currentPlayer == PlayerRole.White) ? PlayerRole.Black : PlayerRole.White;

            if (_currentPlayer == PlayerRole.White)
            {
                ComputePlaceOneChess(new Postion(Rows / 2, Cols / 2));
            }

            UpdateStatus(PlayingState.SelectingCandiate, "Playing. Please select an candidate of your next move.");

        }

        private void PlaceOneChess(Postion p, PointState state)
        {
            if (state != PointState.Black && state != PointState.White)
            {
                return;
            }

            chesses[p.X][p.Y].Source = (state == PointState.Black) ? BlackChessPic.Source : WhiteChessPic.Source;
            chesses[p.X][p.Y].Visibility = Visibility.Visible;
            globalState[p.X][p.Y] = state;
        }

        private void ComputePlaceOneChess(Postion p)
        {
            var state = (_currentPlayer == PlayerRole.Black) ? PointState.White : PointState.Black;
            PlaceOneChess(p, state);
            SetLastStepIndicator(p);

            computerHistory.Add(p);
        }

        private void HumanPlaceOneChess(Postion p)
        {
            var state = (_currentPlayer == PlayerRole.Black) ? PointState.Black : PointState.White;
            PlaceOneChess(p, state);
            humanHistory.Add(p);
            currentCursor++;
            //tbStatus.Text = "Playing. Please select an candidate of your next move.";
        }

        private bool IsValidCursorPostion(Postion p)
        {
            return p.X > -1 && p.X < Rows && p.Y > -1 && p.Y < Cols;
        }

        private void SetLastStepIndicator(Postion p)
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

        private Postion GetPostion(Point p)
        {
            int x = -1;
            int y = -1;

            for (int i = 0; i <= Rows; i++)
            {
                if (StartTop - 0.5 * RowDeta + i * (RowDeta) < p.Y && p.Y <= StartTop + 0.5 * RowDeta + i * (RowDeta))
                {
                    x = i;
                    break;
                }
            }

            for (int j = 0; j <= Cols; j++)
            {
                if (StartLeft - 0.5 * ColDeta + j * ColDeta < p.X && p.X <= StartLeft + 0.5 * ColDeta + j * ColDeta)
                {
                    y = j;
                    break;
                }
            }

            return new Postion(x, y);

        }

        private bool IsOver(Postion p)
        {
            int i, j, t;

            i = p.X;
            j = 0;

            while (i > 0 && globalState[i][p.Y] == globalState[p.X][p.Y])
            {
                i--;
            }

            i++;

            while (i < Rows && globalState[i][p.Y] == globalState[p.X][p.Y])
            {
                j++;
                i++;
            }

            if (j >= 5)
                return true;

            i = p.Y;
            j = 0;
            while (i >= 0 && globalState[p.X][i] == globalState[p.X][p.Y])
            {
                i--;
            }

            i++;

            while (i < Cols && globalState[p.X][i] == globalState[p.X][p.Y])
            {
                j++;
                i++;
            }

            if (j >= 5)
                return true;

            i = p.X;
            t = p.Y;
            j = 0;
            while (i >= 0 && t >= 0 && globalState[i][t] == globalState[p.X][p.Y])
            {
                i--;
                t--;
            }

            i++;
            t++;

            while (i < Rows && t < Cols && globalState[i][t] == globalState[p.X][p.Y])
            {
                i++;
                t++;
                j++;
            }
            if (j >= 5)
                return true;

            i = p.X;
            t = p.Y;
            j = 0;
            while (i >= 0 && t < Cols && globalState[i][t] == globalState[p.X][p.Y])
            {
                i--;
                t++;
            }

            i++;
            t--;
            while (i < Rows && t >= 0 && globalState[i][t] == globalState[p.X][p.Y])
            {
                i++;
                t--;
                j++;
            }
            if (j >= 5)
                return true;

            return false;
        }

        private int num(int i, int j, int tag, ref int sta)
        {
            PointState t = globalState[i][j];
            int sum = 0;
            switch (tag)
            {
                case 0: while (i > 0 && globalState[i][j] == t) i--; i++; sta = i;
                    while (i < Rows && globalState[i][j] == t) { sum++; i++; } break;
                case 1: while (j > 0 && globalState[i][j] == t) j--; j++; sta = j;
                    while (j < Cols && globalState[i][j] == t) { sum++; j++; } break;
                case 2: while (i > 0 && j > 0 && globalState[i][j] == t) { i--; j--; } i++; j++; sta = i;
                    while (i < Rows && j < Cols && globalState[i][j] == t) { sum++; i++; j++; } break;
                case 3: while (i > 0 && j < Cols && globalState[i][j] == t) { i--; j++; } i++; j--; sta = i;
                    while (i < Rows && j > 0 && globalState[i][j] == t) { sum++; i++; j--; } break;
            }
            return sum;
        }

        private bool if5(int a, int b, ref int bestx, ref int besty)
        {
            int sta = 0, sum, sx, sy, ex, ey;
            bool s1, e1, s2, e2, s3, e3, s4, e4;
            //heng
            sum = num(a, b, 0, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            if (sx > 1) s1 = (globalState[sx - 1][b] == PointState.None); else s1 = false;
            if (ex + 1 < Rows) e1 = (globalState[ex + 1][b] == PointState.None); else e1 = false;
            if (sx > 2) s2 = (s1 && (globalState[sx - 2][b] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows) e2 = (e1 && (globalState[ex + 2][b] == globalState[a][b])); else e2 = false;
            if (sx > 3) s3 = (s2 && (globalState[sx - 3][b] == globalState[a][b])); else s3 = false;
            if (ex + 3 < Rows) e3 = (e2 && (globalState[ex + 3][b] == globalState[a][b])); else e3 = false;
            if (sx > 4) s4 = (s3 && (globalState[sx - 4][b] == globalState[a][b])); else s4 = false;
            if (ex + 4 < Rows) e4 = (e3 && (globalState[ex + 4][b] == globalState[a][b])); else e4 = false;
            switch (sum)
            {
                case 4: if (s1) { bestx = sx - 1; besty = b; return true; }
                    if (e1) { bestx = ex + 1; besty = b; return true; }
                    break;
                case 3: if (s2) { bestx = sx - 1; besty = b; return true; }
                    if (e2) { bestx = ex + 1; besty = b; return true; }
                    break;
                case 2: if (s3) { bestx = sx - 1; besty = b; return true; }
                    if (e3) { bestx = ex + 1; besty = b; return true; }
                    break;
                case 1: if (s4) { bestx = sx - 1; besty = b; return true; }
                    if (e4) { bestx = ex + 1; besty = b; return true; }
                    break;
            }
            //heng
            sum = num(a, b, 1, ref sta);
            sy = sta;
            ey = sta + sum - 1;
            if (sy > 1) s1 = (globalState[a][sy - 1] == PointState.None); else s1 = false;
            if (ey + 1 < Cols) e1 = (globalState[a][ey + 1] == PointState.None); else e1 = false;
            if (sy > 2) s2 = (s1 && (globalState[a][sy - 2] == globalState[a][b])); else s2 = false;
            if (ey + 2 < Cols) e2 = (e1 && (globalState[a][ey + 2] == globalState[a][b])); else e2 = false;
            if (sy > 3) s3 = (s2 && (globalState[a][sy - 3] == globalState[a][b])); else s3 = false;
            if (ey + 3 < Cols) e3 = (e2 && (globalState[a][ey + 3] == globalState[a][b])); else e3 = false;
            if (sy > 4) s4 = (s3 && (globalState[a][sy - 4] == globalState[a][b])); else s4 = false;
            if (ey + 4 < Cols) e4 = (e3 && (globalState[a][ey + 4] == globalState[a][b])); else e4 = false;
            switch (sum)
            {
                case 4: if (s1) { bestx = a; besty = sy - 1; return true; }
                    if (e1) { bestx = a; besty = ey + 1; return true; }
                    break;
                case 3: if (s2) { bestx = a; besty = sy - 1; return true; }
                    if (e2) { bestx = a; besty = ey + 1; return true; }
                    break;
                case 2: if (s3) { bestx = a; besty = sy - 1; return true; }
                    if (e3) { bestx = a; besty = ey + 1; return true; }
                    break;
                case 1: if (s4) { bestx = a; besty = sy - 1; return true; }
                    if (e4) { bestx = a; besty = ey + 1; return true; }
                    break;
            }
            //main cross
            sum = num(a, b, 2, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            sy = b - a + sta;
            ey = b - a + sta + sum - 1;
            if (sx > 1 && sy > 1) s1 = (globalState[sx - 1][sy - 1] == PointState.None); else s1 = false;
            if (ex + 1 < Rows && ey + 1 < Cols) e1 = (globalState[ex + 1][ey + 1] == PointState.None); else e1 = false;
            if (sx > 2 && sy > 2) s2 = (s1 && (globalState[sx - 2][sy - 2] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows && ey + 2 < Cols) e2 = (e1 && (globalState[ex + 2][ey + 2] == globalState[a][b])); else e2 = false;
            if (sx > 3 && sy > 3) s3 = (s2 && (globalState[sx - 3][sy - 3] == globalState[a][b])); else s3 = false;
            if (ex + 3 < Rows && ey + 3 < Cols) e3 = (e2 && (globalState[ex + 3][ey + 3] == globalState[a][b])); else e3 = false;
            if (sx > 4 && sy > 4) s4 = (s3 && (globalState[sx - 4][sy - 4] == globalState[a][b])); else s4 = false;
            if (ex + 4 < Rows && ey + 4 < Cols) e4 = (e3 && (globalState[ex + 4][ey + 4] == globalState[a][b])); else e4 = false;
            switch (sum)
            {
                case 4: if (s1) { bestx = sx - 1; besty = sy - 1; return true; }
                    if (e1) { bestx = ex + 1; besty = ey + 1; return true; }
                    break;
                case 3: if (s2) { bestx = sx - 1; besty = sy - 1; return true; }
                    if (e2) { bestx = ex + 1; besty = ey + 1; return true; }
                    break;
                case 2: if (s3) { bestx = sx - 1; besty = sy - 1; return true; }
                    if (e3) { bestx = ex + 1; besty = ey + 1; return true; }
                    break;
                case 1: if (s4) { bestx = sx - 1; besty = sy - 1; return true; }
                    if (e4) { bestx = ex + 1; besty = ey + 1; return true; }
                    break;
            }
            //side cross
            sum = num(a, b, 3, ref sta);

            sx = sta;
            ex = sta + sum - 1;
            sy = b + a - sta;
            ey = b + a - sta - sum + 1;
            if (sx - 1 > 0 && sy + 1 < Cols) s1 = (globalState[sx - 1][sy + 1] == PointState.None); else s1 = false;
            if (ex + 1 < Rows && ey - 1 > 0) e1 = (globalState[ex + 1][ey - 1] == PointState.None); else e1 = false;
            if (sx - 2 > 0 && sy + 2 < Cols) s2 = (s1 && (globalState[sx - 2][sy + 2] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows && ey - 2 > 0) e2 = (e1 && (globalState[ex + 2][ey - 2] == globalState[a][b])); else e2 = false;
            if (sx - 3 > 0 && sy + 3 < Cols) s3 = (s2 && (globalState[sx - 3][sy + 3] == globalState[a][b])); else s3 = false;
            if (ex + 3 < Rows && ey - 3 > 0) e3 = (e2 && (globalState[ex + 3][ey - 3] == globalState[a][b])); else e3 = false;
            if (sx - 4 > 0 && sy + 4 < Cols) s4 = (s3 && (globalState[sx - 4][sy + 4] == globalState[a][b])); else s4 = false;
            if (ex + 4 < Rows && ey - 4 > 0) e4 = (e3 && (globalState[ex + 4][ey - 4] == globalState[a][b])); else e4 = false;
            switch (sum)
            {
                case 4: if (s1) { bestx = sx - 1; besty = sy + 1; return true; }
                    if (e1) { bestx = ex + 1; besty = ey - 1; return true; }
                    break;
                case 3: if (s2) { bestx = sx - 1; besty = sy + 1; return true; }
                    if (e2) { bestx = ex + 1; besty = ey - 1; return true; }
                    break;
                case 2: if (s3) { bestx = sx - 1; besty = sy + 1; return true; }
                    if (e3) { bestx = ex + 1; besty = ey - 1; return true; }
                    break;
                case 1: if (s4) { bestx = sx - 1; besty = sy + 1; return true; }
                    if (e4) { bestx = ex + 1; besty = ey - 1; return true; }
                    break;
            }
            return false;

        }

        private bool ifer(int a, int b, ref int bestx, ref int besty)
        {//
            int m = 0, n = 0;
            int[] pri = new int[] { 6, 6, 6, 5 };
            if (a > 2)
                if (globalState[a - 1][b] == PointState.None)
                {
                    bestx = a - 1; besty = b;
                    globalState[bestx][besty] = globalState[a][b];
                    if (if5(bestx, besty, ref m, ref n))
                        pri[0] = 1;
                    if (if4(bestx, besty, ref m, ref n) > 0)
                        pri[0] = 2;
                    if (if3(bestx, besty, ref m, ref n) > 0)
                        pri[0] = 3;
                    globalState[bestx][besty] = 0;
                }
            if (b > 2)
                if (globalState[a][b - 1] == PointState.None)
                {
                    bestx = a; besty = b - 1;
                    globalState[bestx][besty] = globalState[a][b];
                    if (if5(bestx, besty, ref m, ref n))
                        pri[1] = 1;
                    if (if4(bestx, besty, ref m, ref n) > 0)
                        pri[1] = 2;
                    if (if3(bestx, besty, ref m, ref n) > 0)
                        pri[1] = 3;
                    globalState[bestx][besty] = 0;
                }
            if (b < Cols - 1)
                if (globalState[a][b + 1] == PointState.None)
                {
                    bestx = a; besty = b + 1;
                    globalState[bestx][besty] = globalState[a][b];
                    if (if5(bestx, besty, ref m, ref n))
                        pri[2] = 1;
                    if (if4(bestx, besty, ref m, ref n) > 0)
                        pri[2] = 2;
                    if (if3(bestx, besty, ref m, ref n) > 0)
                        pri[2] = 3;
                    globalState[bestx][besty] = 0;
                }
            if (a < Rows - 1)
                if (globalState[a + 1][b] == PointState.None)
                {
                    bestx = a + 1; besty = b;
                    globalState[bestx][besty] = globalState[a][b];
                    if (if5(bestx, besty, ref m, ref n))
                        pri[3] = 1;
                    if (if4(bestx, besty, ref m, ref n) > 0)
                        pri[3] = 2;
                    if (if3(bestx, besty, ref m, ref n) > 0)
                        pri[3] = 3;
                    globalState[bestx][besty] = 0;
                } int min = 4;
            for (m = 3; m < 4; m++)
            {
                if (pri[m] < min)
                { n = m; min = pri[m]; }
            }
            switch (n)
            {
                case 0: bestx = a - 1; besty = b; return true;
                case 1: bestx = a; besty = b - 1; return true;
                case 2: bestx = a; besty = b + 1; return true;
                case 3: bestx = a + 1; besty = b; return true;
            }
            return false;

        }

        private int if4(int a, int b, ref int bestx, ref int besty)
        {
            int sta = 0, sum, sx, sy, ex, ey, m = 0, n = 0;
            bool s1, e1, s2, e2, s3, e3;

            PPoint[] pot = new PPoint[160];
            for (int k = 0; k < 160; k++)
            {
                pot[k] = new PPoint();
            }

            int tag = -1;
            //Heng
            sum = num(a, b, 0, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            if (sx > 1) s1 = (globalState[sx - 1][b] == PointState.None); else s1 = false;
            if (ex + 1 < Rows) e1 = (globalState[ex + 1][b] == PointState.None); else e1 = false;
            if (sx > 2) s2 = (s1 && (globalState[sx - 2][b] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows) e2 = (e1 && (globalState[ex + 2][b] == globalState[a][b])); else e2 = false;
            if (sx > 3) s3 = (s2 && (globalState[sx - 3][b] == globalState[a][b])); else s3 = false;
            if (ex + 3 < Rows) e3 = (e2 && (globalState[ex + 3][b] == globalState[a][b])); else e3 = false;
            switch (sum)
            {
                case 3: if (s1)
                    {
                        bestx = sx - 1; besty = b;
                        if (e1 && (globalState[sx - 2][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 2][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }

                        }
                    }
                    if (e1)
                    {
                        bestx = ex + 1; besty = b;
                        if (s1 && (globalState[ex + 2][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 2][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 2: if (s2)
                    {
                        bestx = sx - 1; besty = b;
                        if (e1 && (globalState[sx - 3][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 3][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = ex + 1; besty = b;
                        if (s1 && (globalState[ex + 3][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 3][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s3)
                    {
                        bestx = sx - 1; besty = b;
                        if (e1 && (globalState[sx - 4][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 4][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e3)
                    {
                        bestx = ex + 1; besty = b;
                        if (s1 && (globalState[ex + 4][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 4][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            //纵向成
            sum = num(a, b, 1, ref sta);
            sy = sta;
            ey = sta + sum - 1;
            if (sy > 1) s1 = (globalState[a][sy - 1] == PointState.None); else s1 = false;
            if (ey + 1 < Cols) e1 = (globalState[a][ey + 1] == PointState.None); else e1 = false;
            if (sy > 2) s2 = (s1 && (globalState[a][sy - 2] == globalState[a][b])); else s2 = false;
            if (ey + 2 < Cols) e2 = (e1 && (globalState[a][ey + 2] == globalState[a][b])); else e2 = false;
            if (sy > 3) s3 = (s2 && (globalState[a][sy - 3] == globalState[a][b])); else s3 = false;
            if (ey + 3 < Cols) e3 = (e2 && (globalState[a][ey + 3] == globalState[a][b])); else e3 = false;
            switch (sum)
            {
                case 3: if (s1)
                    {
                        bestx = a; besty = sy - 1;
                        if (e1 && (globalState[a][sy - 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][sy - 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }

                    if (e1)
                    {
                        bestx = a; besty = ey + 1;
                        if (s1 && (globalState[a][ey + 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][ey + 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 2: if (s2)
                    {
                        bestx = a; besty = sy - 1;
                        if (e1 && (globalState[a][sy - 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][sy - 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = a; besty = ey + 1;
                        if (s1 && (globalState[a][ey + 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][ey + 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s3)
                    {
                        bestx = a; besty = sy - 1;
                        if (e1 && (globalState[a][sy - 4] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][sy - 4] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e3)
                    {
                        bestx = a; besty = ey + 1;
                        if (s1 && (globalState[a][ey + 4] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][ey + 4] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            //主对角
            sum = num(a, b, 2, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            sy = b - a + sta;
            ey = b - a + sta + sum - 1;
            if (sx > 1 && sy > 1) s1 = (globalState[sx - 1][sy - 1] == PointState.None); else s1 = false;
            if (ex + 1 < Rows && ey + 1 < Cols) e1 = (globalState[ex + 1][ey + 1] == PointState.None); else e1 = false;
            if (sx > 2 && sy > 2) s2 = (s1 && (globalState[sx - 2][sy - 2] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows && ey + 2 < Cols) e2 = (e1 && (globalState[ex + 2][ey + 2] == globalState[a][b])); else e2 = false;
            if (sx > 3 && sy > 3) s3 = (s2 && (globalState[sx - 3][sy - 3] == globalState[a][b])); else s3 = false;
            if (ex + 3 < Rows && ey + 3 < Cols) e3 = (e2 && (globalState[ex + 3][ey + 3] == globalState[a][b])); else e3 = false;
            switch (sum)
            {
                case 3: if (s1)
                    {
                        bestx = sx - 1; besty = sy - 1;
                        if (e1 && (globalState[sx - 2][sy - 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 2][sy - 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e1)
                    {
                        bestx = ex + 1; besty = ey + 1;
                        if (s1 && (globalState[ex + 2][ey + 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 2][ey + 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 2: if (s2)
                    {
                        bestx = sx - 1; besty = sy - 1;
                        if (e1 && (globalState[sx - 3][sy - 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 3][sy - 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = ex + 1; besty = ey + 1;
                        if (s1 && (globalState[ex + 3][ey + 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 3][ey + 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s3)
                    {
                        bestx = sx - 1; besty = sy - 1;
                        if (e1 && (globalState[sx - 4][sy - 4] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 4][sy - 4] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e3)
                    {
                        bestx = ex + 1; besty = ey + 1;
                        if (s1 && (globalState[ex + 4][ey + 4] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 4][ey + 4] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            //副对角
            sum = num(a, b, 3, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            sy = b + a - sta;
            ey = b + a - sta - sum + 1;
            if (sx - 1 > 0 && sy + 1 < Cols) s1 = (globalState[sx - 1][sy + 1] == PointState.None); else s1 = false;
            if (ex + 1 < Rows && ey - 1 > 0) e1 = (globalState[ex + 1][ey - 1] == PointState.None); else e1 = false;
            if (sx - 2 > 0 && sy + 2 < Cols) s2 = (s1 && (globalState[sx - 2][sy + 2] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows && ey - 2 > 0) e2 = (e1 && (globalState[ex + 2][ey - 2] == globalState[a][b])); else e2 = false;
            if (sx - 3 > 0 && sy + 3 < Cols) s3 = (s2 && (globalState[sx - 3][sy + 3] == globalState[a][b])); else s3 = false;
            if (ex + 3 < Rows && ey - 3 > 0) e3 = (e2 && (globalState[ex + 3][ey - 3] == globalState[a][b])); else e3 = false;
            switch (sum)
            {
                case 3: if (s1)
                    {
                        bestx = sx - 1; besty = sy + 1;
                        if (e1 && (sy + 2 < Cols && globalState[sx - 2][sy + 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (sy + 2<Cols && globalState[sx - 2][sy + 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e1)
                    {
                        bestx = ex + 1; besty = ey - 1;
                        if (s1 && (globalState[ex + 2][ey - 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 2][ey - 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    break;
                case 2: if (s2)
                    {
                        bestx = sx - 1; besty = sy + 1;
                        if (e1 && (globalState[sx - 3][sy + 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 3][sy + 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = ex + 1; besty = ey - 1;
                        if (s1 && (globalState[ex + 3][ey - 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 3][ey - 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    break;
                case 1: if (s3)
                    {
                        bestx = sx - 1; besty = sy + 1;
                        if (e1 && (globalState[sx - 4][sy + 4] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 4][sy + 4] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e3)
                    {
                        bestx = ex + 1; besty = ey - 1;
                        if (s1 && (globalState[ex + 4][ey - 4] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 4][ey - 4] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    break;

            }
            int i, min = 3;
            for (i = 0; i < tag; i++)
            {
                if (pot[i].pri < min)
                {
                    min = pot[i].pri;
                    bestx = pot[i].x;
                    besty = pot[i].y;
                }
            }
            return min;
        }

        private int if3(int a, int b, ref int bestx, ref int besty)
        {
            int sta = 0, sum, sx, sy, ex, ey, m = 0, n = 0;
            bool s1, e1, s2, e2;
            PPoint[] pot = new PPoint[160];
            for (int k = 0; k < 160; k++)
            {
                pot[k] = new PPoint();
            }
            int tag = -1;
            //Heng
            sum = num(a, b, 0, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            if (sx > 1) s1 = (globalState[sx - 1][b] == PointState.None); else s1 = false;
            if (ex + 1 < Rows) e1 = (globalState[ex + 1][b] == PointState.None); else e1 = false;
            if (sx > 2) s2 = (s1 && (globalState[sx - 2][b] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows) e2 = (e1 && (globalState[ex + 2][b] == globalState[a][b])); else e2 = false;
            switch (sum)
            {
                case 2: if (s1)
                    {
                        bestx = sx - 1; besty = b;
                        if (e1 && (globalState[sx - 2][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 2][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e1)
                    {
                        bestx = ex + 1; besty = b;
                        if (s1 && (globalState[ex + 2][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 2][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s2)
                    {
                        bestx = sx - 1; besty = b;
                        if (e1 && (globalState[sx - 3][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 3][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = ex + 1; besty = b;
                        if (s1 && (globalState[ex + 3][b] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 3][b] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            //shu
            sum = num(a, b, 1, ref sta);
            sy = sta;
            ey = sta + sum - 1;
            if (sy > 1) s1 = (globalState[a][sy - 1] == PointState.None); else s1 = false;
            if (ey + 1 < Cols) e1 = (globalState[a][ey + 1] == PointState.None); else e1 = false;
            if (sy > 2) s2 = (s1 && (globalState[a][sy - 2] == globalState[a][b])); else s2 = false;
            if (ey + 2 < Cols) e2 = (e1 && (globalState[a][ey + 2] == globalState[a][b])); else e2 = false;
            switch (sum)
            {
                case 2: if (s1)
                    {
                        bestx = a; besty = sy - 1;
                        if (e1 && (globalState[a][sy - 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][sy - 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e1)
                    {
                        bestx = a; besty = ey + 1;
                        if (s1 && (globalState[a][ey + 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][ey + 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s2)
                    {
                        bestx = a; besty = sy - 1;
                        if (e1 && (globalState[a][sy - 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][sy - 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = a; besty = ey + 1;
                        if (s1 && (globalState[a][ey + 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[a][ey + 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            //主对角
            sum = num(a, b, 2, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            sy = b - a + sta;
            ey = b - a + sta + sum - 1;
            if (sx > 1 && sy > 1) s1 = (globalState[sx - 1][sy - 1] == PointState.None); else s1 = false;
            if (ex + 1 < Rows && ey + 1 < Cols) e1 = (globalState[ex + 1][ey + 1] == PointState.None); else e1 = false;
            if (sx > 2 && sy > 2) s2 = (s1 && (globalState[sx - 2][sy - 2] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows && ey + 2 < Cols) e2 = (e1 && (globalState[ex + 2][ey + 2] == globalState[a][b])); else e2 = false;
            switch (sum)
            {
                case 2: if (s1)
                    {
                        bestx = sx - 1; besty = sy - 1;
                        if (e1 && (globalState[sx - 2][sy - 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 2][sy - 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e1)
                    {
                        bestx = ex + 1; besty = ey + 1;
                        if (s1 && (globalState[ex + 2][ey + 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 2][ey + 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s2)
                    {
                        bestx = sx - 1; besty = sy - 1;
                        if (e1 && (globalState[sx - 3][sy - 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 3][sy - 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = ex + 1; besty = ey + 1;
                        if (s1 && (globalState[ex + 3][ey + 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 3][ey + 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            //副对角
            sum = num(a, b, 3, ref sta);
            sx = sta;
            ex = sta + sum - 1;
            sy = b + a - sta;
            ey = b + a - sta - sum + 1;
            if (sx - 1 > 0 && sy + 1 < Cols) s1 = (globalState[sx - 1][sy + 1] == PointState.None); else s1 = false;
            if (ex + 1 < Rows && ey - 1 > 0) e1 = (globalState[ex + 1][ey - 1] == PointState.None); else e1 = false;
            if (sx - 2 > 0 && sy + 2 < Cols) s2 = (s1 && (globalState[sx - 2][sy + 2] == globalState[a][b])); else s2 = false;
            if (ex + 2 < Rows && ey - 2 > 0) e2 = (e1 && (globalState[ex + 2][ey - 2] == globalState[a][b])); else e2 = false;
            switch (sum)
            {
                case 2: if (s1)
                    {
                        bestx = sx - 1; besty = sy + 1;
                        if (e1 && (globalState[sx - 2][sy + 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 2][sy + 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e1)
                    {
                        bestx = ex + 1; besty = ey - 1;
                        if (s1 && (globalState[ex + 2][ey - 2] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[ex + 2][ey - 2] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
                case 1: if (s2)
                    {
                        bestx = sx - 1; besty = sy + 1;
                        if (e1 && (globalState[sx - 3][sy + 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (globalState[sx - 3][sy + 3] == PointState.None))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    }
                    if (e2)
                    {
                        bestx = ex + 1; besty = ey - 1;
                        if (s1 && ex + 3 < Rows && (globalState[ex + 3][ey - 3] == PointState.None))
                            return 0;
                        else
                        {
                            if (e1 || (ex + 3 < Rows && (globalState[ex + 3][ey - 3] == PointState.None)))
                            {
                                globalState[bestx][besty] = globalState[a][b];
                                if (if5(bestx, besty, ref m, ref n))
                                { globalState[bestx][besty] = PointState.None; return 0; }
                                if (if4(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 1;
                                }
                                else if (if3(bestx, besty, ref m, ref n) == 0)
                                {
                                    tag++;
                                    pot[tag].x = bestx;
                                    pot[tag].y = besty;
                                    pot[tag].pri = 2;
                                }
                                globalState[bestx][besty] = PointState.None;
                            }
                        }
                    } break;
            }
            int i, min = 3;
            for (i = 0; i < tag; i++)
            {
                if (pot[i].pri < min)
                {
                    min = pot[i].pri;
                    bestx = pot[i].x;
                    besty = pot[i].y;
                }
            }
            return min;
        }
    }
}