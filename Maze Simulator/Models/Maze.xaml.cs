using Maze_Simulator.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Maze_Simulator.Models
{
    /// <summary>
    /// Interaction logic for Maze.xaml
    /// </summary>
    public partial class Maze : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private readonly Random rand = new();

        private Matrix<Cell> matrix = new();

        private MazeState state;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public bool HasColor
        {
            get => Cell.HasColor;
            set
            {
                Cell.HasColor = value;
                for (int i = 0; i < matrix.Row; i++)
                {
                    for (int j = 0; j < matrix.Column; j++)
                    {
                        matrix[i, j].SetColor();
                    }
                }
            }
        }

        public int DelayTime { get; set; }

        public MazeState State
        {
            get => state;
            private set
            {
                state = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public Maze()
        {
            InitializeComponent();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            double size = Math.Min(sizeInfo.NewSize.Width / matrix.Column, sizeInfo.NewSize.Height / matrix.Row);
            for (int i = 0; i < matrix.Row; i++)
            {
                for (int j = 0; j < matrix.Column; j++)
                {
                    matrix[i, j].Size = size;
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Check state of maze

        public bool IsEmpty()
        {
            return State == MazeState.Empty;
        }

        public bool IsRunning()
        {
            return State == MazeState.Running;
        }

        public bool HasPath()
        {
            return State == MazeState.HasPath;
        }

        public bool IsCompleted()
        {
            return State == MazeState.Completed;
        }

        #endregion

        #region Auxiliary methods

        public void SetSize(int row, int column)
        {
            matrix = new(row, column);
            double size = Math.Min(ActualWidth / column, ActualHeight / row);

            grid.Children.Clear();
            grid.Rows = row;
            grid.Columns = column;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    matrix[i, j] = new Cell()
                    {
                        Size = size,
                        Row = i,
                        Column = j
                    };

                    grid.Children.Add(matrix[i, j]);
                }
            }

            State = MazeState.Empty;
            Cell.Start = null;
            Cell.End = null;
            Cell.HasPath = false;
        }

        public void Reset()
        {
            Refresh();

            if (Cell.HasStart())
            {
                Cell.Start.Point = CellPoint.None;
            }

            if (Cell.HasEnd())
            {
                Cell.End.Point = CellPoint.None;
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < matrix.Row; i++)
            {
                for (int j = 0; j < matrix.Column; j++)
                {
                    if (!Cell.IsStartOrEnd(matrix[i, j]))
                    {
                        matrix[i, j].Point = CellPoint.None;
                    }
                    matrix[i, j].State = CellState.Created;
                }
            }

            Cell.HasPath = false;
            State = MazeState.Completed;
        }

        private Cell RandomCell()
        {
            int row = rand.Next(matrix.Row);
            int column = rand.Next(matrix.Column);
            return matrix[row, column];
        }

        private void Connect(Cell cell1, Cell cell2)
        {
            if (cell1.Row < cell2.Row)
            {
                cell1.RemoveWall(Wall.South);
                cell2.RemoveWall(Wall.North);
                return;
            }

            if (cell1.Row > cell2.Row)
            {
                cell1.RemoveWall(Wall.North);
                cell2.RemoveWall(Wall.South);
                return;
            }

            if (cell1.Column < cell2.Column)
            {
                cell1.RemoveWall(Wall.East);
                cell2.RemoveWall(Wall.West);
                return;
            }

            if (cell1.Column > cell2.Column)
            {
                cell1.RemoveWall(Wall.West);
                cell2.RemoveWall(Wall.East);
                return;
            }
        }

        private bool IsConnected(Cell cell1, Cell cell2)
        {
            if (cell1.Row == cell2.Row && cell1.Column == cell2.Column)
            {
                return false;
            }

            if (cell1.Row == cell2.Row)
            {
                if (cell1.Column < cell2.Column && !cell1.HasWall(Wall.East) && !cell2.HasWall(Wall.West))
                {
                    return true;
                }

                if (cell1.Column > cell2.Column && !cell1.HasWall(Wall.West) && !cell2.HasWall(Wall.East))
                {
                    return true;
                }
            }

            if (cell1.Column == cell2.Column)
            {
                if (cell1.Row < cell2.Row && !cell1.HasWall(Wall.South) && !cell2.HasWall(Wall.North))
                {
                    return true;
                }

                if (cell1.Row > cell2.Row && !cell1.HasWall(Wall.North) && !cell2.HasWall(Wall.South))
                {
                    return true;
                }
            }

            return false;
        }

        private List<Cell> GetNeighbors(Cell cell, CellState state)
        {
            List<Cell> cells = new();
            var (row, column) = (cell.Row, cell.Column);

            if (row - 1 >= 0)
            {
                if (matrix[row - 1, column].State == state)
                {
                    cells.Add(matrix[row - 1, column]);
                }
            }

            if (row + 1 < matrix.Row)
            {
                if (matrix[row + 1, column].State == state)
                {
                    cells.Add(matrix[row + 1, column]);
                }
            }

            if (column - 1 >= 0)
            {
                if (matrix[row, column - 1].State == state)
                {
                    cells.Add(matrix[row, column - 1]);
                }
            }

            if (column + 1 < matrix.Column)
            {
                if (matrix[row, column + 1].State == state)
                {
                    cells.Add(matrix[row, column + 1]);
                }
            }

            return cells;
        }

        private void SetStateNeighbors(List<Cell> neighbors, CellState state)
        {
            foreach (var item in neighbors)
            {
                item.State = state;
            }
        }

        private List<Cell> GetConnectedNeighbors(Cell cell)
        {
            var neighbors = GetNeighbors(cell, CellState.Created);
            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (IsConnected(cell, neighbors[i]))
                {
                    neighbors[i].State = CellState.Visiting;
                }
                else
                {
                    neighbors.RemoveAt(i);
                }
            }
            return neighbors;
        }

        private void GetBucket(List<Cell>[] bucket, List<Linker> linkers)
        {
            int id = 0;
            for (int i = 0; i < matrix.Row; i++)
            {
                for (int j = 0; j < matrix.Column; j++, id++)
                {
                    matrix[i, j].Id = id;
                    bucket[id].Add(matrix[i, j]);

                    if (i + 1 < matrix.Row)
                    {
                        linkers.Add(new Linker(matrix[i, j], matrix[i + 1, j]));
                    }

                    if (j + 1 < matrix.Column)
                    {
                        linkers.Add(new Linker(matrix[i, j], matrix[i, j + 1]));
                    }
                }
            }
        }

        private void MergeBucket(List<Cell>[] bucket, int startId, int endId)
        {
            if (startId == endId)
            {
                return;
            }

            foreach (var item in bucket[startId])
            {
                item.Id = endId;
                bucket[endId].Add(item);
            }

            bucket[startId].Clear();
        }

        private void SetDistance()
        {
            for (int i = 0; i < matrix.Row; i++)
            {
                for (int j = 0; j < matrix.Column; j++)
                {
                    matrix[i, j].Distance = int.MaxValue;
                    matrix[i, j].RootDistance = int.MaxValue;
                    matrix[i, j].ManhattanDistance = 2 * (Math.Abs(i - Cell.End.Row) + Math.Abs(j - Cell.End.Column));
                }
            }
        }

        private async Task GetPath()
        {
            var cell = Cell.End.CellParent;
            while (!Cell.IsStart(cell))
            {
                await Task.Delay(DelayTime);

                cell.Point = CellPoint.Path;
                cell = cell.CellParent;
            }
        }

        #endregion

        #region Maze generator

        public async Task DfsGenerator()
        {
            State = MazeState.Running;

            Stack<Cell> path = new();

            var cell = RandomCell();
            path.Push(cell);

            while (path.Count != 0)
            {
                cell = path.Peek();
                cell.State = CellState.Visited;

                await Task.Delay(DelayTime);

                var neighbor = GetNeighbors(cell, CellState.None);
                if (neighbor.Count != 0)
                {
                    var c = neighbor[rand.Next(neighbor.Count)];
                    Connect(cell, c);
                    c.Depth = cell.Depth + 1;
                    path.Push(c);

                    cell.State = CellState.Visiting;
                }
                else
                {
                    path.Pop();
                    cell.State = CellState.Created;
                }
            }

            State = MazeState.Completed;
        }

        public async Task BfsGenerator()
        {
            State = MazeState.Running;

            List<Cell> path = new();

            var cell = RandomCell();
            path.Add(cell);

            while (path.Count != 0)
            {
                int pos = rand.Next(path.Count);
                cell = path[pos];
                cell.State = CellState.Visited;

                await Task.Delay(DelayTime);

                var neighbors = GetNeighbors(cell, CellState.Created);
                if (neighbors.Count != 0)
                {
                    var c = neighbors[rand.Next(neighbors.Count)];
                    cell.Depth = c.Depth + 1;
                    Connect(cell, c);
                }

                neighbors = GetNeighbors(cell, CellState.None);
                SetStateNeighbors(neighbors, CellState.Visiting);
                path.AddRange(neighbors);

                cell.State = CellState.Created;
                path.RemoveAt(pos);
            }

            State = MazeState.Completed;
        }

        public async Task KruskalGenerator()
        {
            State = MazeState.Running;

            List<Cell>[] bucket = new List<Cell>[matrix.Size];
            for (int i = 0; i < matrix.Size; i++)
            {
                bucket[i] = new List<Cell>();
            }
            List<Linker> linkers = new();
            GetBucket(bucket, linkers);

            while (linkers.Count != 0)
            {
                int pos = rand.Next(linkers.Count);
                var linker = linkers[pos];

                if (linker.Start.State == CellState.Created && linker.End.State != CellState.Created)
                {
                    linker.End.Depth = linker.Start.Depth + 1;
                }
                else if (linker.End.State == CellState.Created && linker.Start.State != CellState.Created)
                {
                    linker.Start.Depth = linker.End.Depth + 1;
                }

                linker.Start.State = CellState.Visited;
                linker.End.State = CellState.Visited;

                await Task.Delay(DelayTime);

                int startId = linker.Start.Id;
                int endId = linker.End.Id;
                if (startId != endId)
                {
                    Connect(linker.Start, linker.End);
                    MergeBucket(bucket, startId, endId);
                }

                linker.Start.State = CellState.Created;
                linker.End.State = CellState.Created;

                linkers.RemoveAt(pos);
            }

            State = MazeState.Completed;
        }

        public async Task PrimGenerator()
        {
            State = MazeState.Running;

            List<Cell> path = new();

            var cell = RandomCell();
            path.Add(cell);

            while (path.Count != 0)
            {
                int minWeight = path.Min(c => c.Weight);
                cell = path.Find(c => c.Weight == minWeight);
                cell.State = CellState.Visited;

                await Task.Delay(DelayTime);

                var neighbors = GetNeighbors(cell, CellState.Created);
                if (neighbors.Count != 0)
                {
                    var c = neighbors[rand.Next(neighbors.Count)];
                    cell.Depth = c.Depth + 1;
                    Connect(cell, c);
                }

                neighbors = GetNeighbors(cell, CellState.None);
                SetStateNeighbors(neighbors, CellState.Visiting);
                path.AddRange(neighbors);

                cell.State = CellState.Created;
                path.Remove(cell);
            }

            State = MazeState.Completed;
        }

        #endregion

        #region Maze solver

        public async Task DfsSolver()
        {
            State = MazeState.Running;

            Stack<Cell> path = new();
            path.Push(Cell.Start);
            while (path.Count != 0)
            {
                Cell cell = path.Pop();
                cell.State = CellState.Visited;
                if (cell == Cell.End)
                {
                    break;
                }

                await Task.Delay(DelayTime);
                var neighbors = GetConnectedNeighbors(cell);
                foreach (var item in neighbors)
                {
                    item.CellParent = cell;
                    path.Push(item);
                }
            }

            await GetPath();

            Cell.HasPath = true;
            State = MazeState.HasPath;
        }

        public async Task BfsSolver()
        {
            State = MazeState.Running;

            Queue<Cell> path = new();
            path.Enqueue(Cell.Start);
            while (path.Count != 0)
            {
                Cell cell = path.Dequeue();
                cell.State = CellState.Visited;
                if (cell == Cell.End)
                {
                    break;
                }

                await Task.Delay(DelayTime);
                var neighbors = GetConnectedNeighbors(cell);
                foreach (var item in neighbors)
                {
                    item.CellParent = cell;
                    path.Enqueue(item);
                }
            }

            await GetPath();

            Cell.HasPath = true;
            State = MazeState.HasPath;
        }

        public async Task DijkstraSolver()
        {
            State = MazeState.Running;

            SetDistance();
            PriorityQueue<Cell> path = new(true);

            Cell.Start.Distance = 0;
            path.Enqueue(Cell.Start, 0);
            while (path.Count != 0)
            {
                var cell = path.Dequeue();
                cell.State = CellState.Visited;
                if (cell == Cell.End)
                {
                    break;
                }

                await Task.Delay(DelayTime);
                var neighbors = GetConnectedNeighbors(cell);
                foreach (var item in neighbors)
                {
                    int minDistance = Math.Min(item.Distance, cell.Distance + 1);

                    if (item.Distance != minDistance)
                    {
                        item.Distance = minDistance;
                        item.CellParent = cell;

                        if (path.Contains(item))
                        {
                            path.UpdatePriority(item, minDistance);
                        }
                    }

                    if (!path.Contains(item))
                    {
                        path.Enqueue(item, item.Distance);
                    }
                }
            }

            await GetPath();

            Cell.HasPath = true;
            State = MazeState.HasPath;
        }

        public async Task AStarSolver()
        {
            State = MazeState.Running;

            SetDistance();
            PriorityQueue<Cell> path = new(true);

            Cell.Start.RootDistance = 0;
            path.Enqueue(Cell.Start, 0);
            while (path.Count != 0)
            {
                var cell = path.Dequeue();
                cell.State = CellState.Visited;
                if (cell == Cell.End)
                {
                    break;
                }

                await Task.Delay(DelayTime);
                var neighbors = GetConnectedNeighbors(cell);
                foreach (var item in neighbors)
                {
                    item.RootDistance = Math.Min(item.RootDistance, cell.RootDistance + 1);
                    int minDistance = Math.Min(item.Distance, item.RootDistance + item.ManhattanDistance);

                    if (item.Distance != minDistance)
                    {
                        item.Distance = minDistance;
                        item.CellParent = cell;

                        if (path.Contains(item))
                        {
                            path.UpdatePriority(item, minDistance);
                        }
                    }

                    if (!path.Contains(item))
                    {
                        path.Enqueue(item, item.Distance);
                    }
                }
            }

            await GetPath();

            Cell.HasPath = true;
            State = MazeState.HasPath;
        }

        #endregion
    }
}
