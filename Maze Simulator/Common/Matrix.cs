using System;
using System.Windows;

namespace Maze_Simulator.Common
{
    public class Matrix<T>
    {
        private int row;

        private int column;

        private T[,] mat;

        public int Row => row;

        public int Column => column;

        public int Size => row * column;

        public T this[int row, int column]
        {
            get => mat[row, column];
            set => mat[row, column] = value;
        }

        public Matrix() : this(0, 0)
        {

        }

        public Matrix(int row, int column)
        {
            this.row = row;
            this.column = column;

            mat = new T[row, column];
        }

        public void AddRow()
        {
            AddRow(Row - 1);
        }

        public void AddRow(int row)
        {
            T[,] newMat = new T[Row + 1, Column];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (i < row)
                    {
                        newMat[i, j] = mat[i, j];
                    }
                    else
                    {
                        newMat[i + 1, j] = mat[i, j];
                    }
                }
            }

            ++this.row;
            mat = newMat;
        }

        public void AddColumn()
        {
            AddColumn(Column - 1);
        }

        public void AddColumn(int column)
        {
            T[,] newMat = new T[Row, Column + 1];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (j < column)
                    {
                        newMat[i, j] = mat[i, j];
                    }
                    else
                    {
                        newMat[i, j + 1] = mat[i, j];
                    }
                }
            }

            ++this.column;
            mat = newMat;
        }

        public void RemoveRow()
        {
            RemoveRow(Row - 1);
        }

        public void RemoveRow(int row)
        {
            T[,] newMat = new T[Row - 1, Column];
            for (int i = 0; i < Row - 1; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (i < row)
                    {
                        newMat[i, j] = mat[i, j];
                    }
                    else
                    {
                        newMat[i, j] = mat[i + 1, j];
                    }
                }
            }

            --this.row;
            mat = newMat;
        }

        public void RemoveColumn()
        {
            RemoveColumn(Column - 1);
        }

        public void RemoveColumn(int column)
        {
            T[,] newMat = new T[Row, Column - 1];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column - 1; j++)
                {
                    if (j < column)
                    {
                        newMat[i, j] = mat[i, j];
                    }
                    else
                    {
                        newMat[i, j] = mat[i, j + 1];
                    }
                }
            }

            --this.column;
            mat = newMat;
        }

        public void Clear()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    this[i, j] = default;
                }
            }
        }

        public void Resize(int row, int column)
        {
            T[,] newMat = new T[row, column];
            for (int i = 0; i < Math.Min(row, Row); i++)
            {
                for (int j = 0; j < Math.Min(column, Column); j++)
                {
                    newMat[i, j] = mat[i, j];
                }
            }

            this.row = row;
            this.column = column;
            mat = newMat;
        }

        public (int row, int column) Find(T value)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (this[i, j].Equals(value))
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }

        public (int row, int column) Find(Predicate<T> match)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (match(this[i, j]))
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }
    }
}
