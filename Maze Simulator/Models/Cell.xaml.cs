using Maze_Simulator.Common;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Maze_Simulator.Models
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>
    public partial class Cell : UserControl
    {
        #region Fields

        private Wall wall = Wall.Full;

        private CellState state;

        private CellPoint point;

        #endregion

        #region Properties

        public Cell CellParent { get; set; }

        public double Size
        {
            get => Width;
            set
            {
                Width = value;
                Height = value;
            }
        }

        public int Id { get; set; }

        public int Row { get; init; }

        public int Column { get; init; }

        public int Weight { get; init; }

        public int Depth { get; set; }

        #region Distance

        public int Distance { get; set; }

        public int RootDistance { get; set; }

        public int ManhattanDistance { get; set; }

        #endregion

        public Brush Color
        {
            get => Background;
            set => Background = value;
        }

        public CellState State
        {
            get => state;
            set
            {
                state = value;
                SetColor();
            }
        }

        public CellPoint Point
        {
            get => point;
            set
            {
                SetPoint(value);
                SetColor();

                RaiseStaticPropertyChanged();
            }
        }

        #endregion

        #region Delegate commands

        public DelegateCommand ResetCommand { get; private set; }

        public DelegateCommand ChangeStartCommand { get; private set; }

        public DelegateCommand ChangeEndCommand { get; private set; }

        #endregion

        public Cell()
        {
            InitializeComponent();

            Weight = new Random().Next(100);
            DataContext = this;
            StaticPropertyChanged += Cell_StaticPropertyChanged;
            SetCommand();
        }

        private void Cell_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ResetCommand.RaiseCanExecuteChanged();
            ChangeStartCommand.RaiseCanExecuteChanged();
            ChangeEndCommand.RaiseCanExecuteChanged();
        }

        private void SetCommand()
        {
            ResetCommand = new(OnReset, CanChange);
            ChangeStartCommand = new(OnChangeStart, CanChange);
            ChangeEndCommand = new(OnChangeEnd, CanChange);
        }

        private void SetPoint(CellPoint value)
        {
            switch (value)
            {
                case CellPoint.None:
                    if (IsStart(this))
                    {
                        Start = null;
                    }

                    if (IsEnd(this))
                    {
                        End = null;
                    }
                    break;

                case CellPoint.Start:
                    Point = CellPoint.None;
                    if (HasStart())
                    {
                        Start.Point = CellPoint.None;
                    }
                    Start = this;
                    break;

                case CellPoint.End:
                    Point = CellPoint.None;
                    if (HasEnd())
                    {
                        End.Point = CellPoint.None;
                    }
                    End = this;
                    break;
            }

            point = value;
        }

        public void SetColor()
        {
            Color = state switch
            {
                CellState.None => Brushes.Gray,
                CellState.Visiting => Brushes.DarkGray,
                CellState.Visited => Brushes.LightGray,
                CellState.Created => HasColor ? new SolidColorBrush(GetColor()) : Brushes.White,
                _ => throw new ArgumentOutOfRangeException()
            };

            Color = point switch
            {
                CellPoint.None => Color,
                CellPoint.Start => Brushes.IndianRed,
                CellPoint.End => Brushes.DeepSkyBlue,
                CellPoint.Path => Brushes.LimeGreen,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void RemoveWall(Wall wall)
        {
            this.wall &= ~wall;

            double left = BorderThickness.Left;
            double top = BorderThickness.Top;
            double right = BorderThickness.Right;
            double bottom = BorderThickness.Bottom;

            switch (wall)
            {
                case Wall.North:
                    top = 0;
                    break;

                case Wall.South:
                    bottom = 0;
                    break;

                case Wall.East:
                    right = 0;
                    break;

                case Wall.West:
                    left = 0;
                    break;
            }

            BorderThickness = new Thickness(left, top, right, bottom);
        }

        public bool HasWall(Wall wall)
        {
            return this.wall.HasFlag(wall);
        }

        private Color GetColor()
        {
            double h = (double)Depth / 256 * 360 % 360;
            return Extension.ColorFromHSV(h, 0.4, 1);
        }

        #region Command

        private void OnReset(object obj)
        {
            Point = CellPoint.None;
        }

        private void OnChangeStart(object obj)
        {
            Point = CellPoint.Start;
        }

        private void OnChangeEnd(object obj)
        {
            Point = CellPoint.End;
        }

        private bool CanChange(object obj)
        {
            return state == CellState.Created && !HasPath;
        }

        #endregion

        #region Static fields, properties, methods

        private static Cell start;

        private static Cell end;

        private static bool hasPath;

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        public static Cell Start
        {
            get => start;
            set
            {
                start = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static Cell End
        {
            get => end;
            set
            {
                end = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool HasPath
        {
            get => hasPath;
            set
            {
                hasPath = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool HasColor { get; set; }

        private static void RaiseStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(typeof(Cell), new PropertyChangedEventArgs(propertyName));
        }

        public static bool HasStartAndEnd()
        {
            return HasStart() && HasEnd();
        }

        public static bool HasStart()
        {
            return start is not null;
        }

        public static bool HasEnd()
        {
            return end is not null;
        }

        public static bool IsStartOrEnd(Cell cell)
        {
            return IsStart(cell) || IsEnd(cell);
        }

        public static bool IsStart(Cell cell)
        {
            return ReferenceEquals(start, cell);
        }

        public static bool IsEnd(Cell cell)
        {
            return ReferenceEquals(end, cell);
        }

        #endregion
    }
}
