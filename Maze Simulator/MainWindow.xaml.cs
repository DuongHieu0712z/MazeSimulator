using Maze_Simulator.Common;
using Maze_Simulator.Models;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Maze_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            maze.PropertyChanged += Maze_PropertyChanged;
            Cell.StaticPropertyChanged += Maze_PropertyChanged;
            SetCommand();
        }

        #region Delegate commands

        public DelegateCommand NewCommand { get; private set; }

        public DelegateCommand DfsGenCommand { get; private set; }

        public DelegateCommand BfsGenCommand { get; private set; }

        public DelegateCommand KruskalGenCommand { get; private set; }

        public DelegateCommand PrimGenCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand DfsSolveCommand { get; private set; }

        public DelegateCommand BfsSolveCommand { get; private set; }

        public DelegateCommand DijkstraSolveCommand { get; private set; }

        public DelegateCommand AStarSolveCommand { get; private set; }

        #endregion

        private void SetCommand()
        {
            NewCommand = new(OnNew, CanNew);

            DfsGenCommand     = new(OnDfsGenerate, CanGenerate);
            BfsGenCommand     = new(OnBfsGenerate, CanGenerate);
            KruskalGenCommand = new(OnKruskalGenerate, CanGenerate);
            PrimGenCommand    = new(OnPrimGenerate, CanGenerate);

            ResetCommand   = new(OnReset, CanResetAndRefresh);
            RefreshCommand = new(OnRefresh, CanResetAndRefresh);

            DfsSolveCommand      = new(OnDfsSolve, CanSolve);
            BfsSolveCommand      = new(OnBfsSolve, CanSolve);
            DijkstraSolveCommand = new(OnDijkstraSolve, CanSolve);
            AStarSolveCommand    = new(OnAStarSolve, CanSolve);
        }

        private void Maze_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NewCommand.RaiseCanExecuteChanged();

            DfsGenCommand.RaiseCanExecuteChanged();
            BfsGenCommand.RaiseCanExecuteChanged();
            KruskalGenCommand.RaiseCanExecuteChanged();
            PrimGenCommand.RaiseCanExecuteChanged();

            ResetCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();

            DfsSolveCommand.RaiseCanExecuteChanged();
            BfsSolveCommand.RaiseCanExecuteChanged();
            DijkstraSolveCommand.RaiseCanExecuteChanged();
            AStarSolveCommand.RaiseCanExecuteChanged();
        }

        #region Handle events

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace((sender as TextBox).Text))
            {
                (sender as TextBox).Text = "0";
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider)
            {
                maze.DelayTime = (int)slider.Value;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            maze.HasColor = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            maze.HasColor = false;
        }

        #endregion

        #region Command

        #region New command

        private void OnNew(object obj)
        {
            int row = int.Parse(txtRow.Text);
            int column = int.Parse(txtColumn.Text);
            maze.SetSize(row, column);
        }

        private bool CanNew(object obj)
        {
            return !maze.IsRunning();
        }

        #endregion

        #region Generate command

        private async void OnDfsGenerate(object obj)
        {
            await maze.DfsGenerator();
        }

        private async void OnBfsGenerate(object obj)
        {
            await maze.BfsGenerator();
        }

        private async void OnKruskalGenerate(object obj)
        {
            await maze.KruskalGenerator();
        }

        private async void OnPrimGenerate(object obj)
        {
            await maze.PrimGenerator();
        }

        private bool CanGenerate(object obj)
        {
            return maze.IsEmpty();
        }

        #endregion

        #region Reset and refresh command

        private void OnReset(object obj)
        {
            maze.Reset();
        }

        private void OnRefresh(object obj)
        {
            maze.Refresh();
        }

        private bool CanResetAndRefresh(object obj)
        {
            return (maze.IsCompleted() || maze.HasPath()) && !maze.IsRunning();
        }

        #endregion

        #region Solve command

        private async void OnDfsSolve(object obj)
        {
            await maze.DfsSolver();
        }

        private async void OnBfsSolve(object obj)
        {
            await maze.BfsSolver();
        }

        private async void OnDijkstraSolve(object obj)
        {
            await maze.DijkstraSolver();
        }

        private async void OnAStarSolve(object obj)
        {
            await maze.AStarSolver();
        }

        private bool CanSolve(object obj)
        {
            return maze.IsCompleted() && !maze.HasPath() && Cell.HasStartAndEnd();
        }

        #endregion

        #endregion
    }
}
