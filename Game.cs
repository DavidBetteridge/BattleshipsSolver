using System;
using System.Collections.Generic;

namespace BattleshipSolver
{
    public class Game
    {
        private readonly int[] _columnCounts;
        private readonly int[] _rowCounts;
        private readonly CellType[,] _state;

        public enum CellType
        {
            Unknown = 0,
            Water = 1,
            SouthEnd = 2,
            NorthEnd = 3,
            WestEnd = 4,
            EastEnd = 5,
            VerticalMiddle = 6,
            HorizontalMiddle = 7,
            Round = 8,
            UnknownBoatPart = 9
        }

        public int NumberOfColumns { get; }
        public int NumberOfRows { get; }

        public Game(int[] columnCounts, int[] rowCounts, CellType[,] initialState)
        {
            NumberOfColumns = initialState.GetUpperBound(0) + 1;
            NumberOfRows = initialState.GetUpperBound(1) + 1;
            _columnCounts = columnCounts;
            _rowCounts = rowCounts;
            _state = initialState;
        }

        internal int ColumnCount(int columnNumber) => _columnCounts[columnNumber];
        internal int RowCount(int rowNumber) => _rowCounts[rowNumber];
        internal CellType CellContents(int column, int row) => _state[column, row];

        internal Solution Solve()
        {
            return SolveColumnsUsingTheColumnCounts();
        }

        private Solution SolveColumnsUsingTheColumnCounts()
        {
            for (int column = 0; column < NumberOfColumns; column++)
            {
                var unknownCellsInColumn = GetUnknownCellsInColumn(column);
                var numberOfBoatsInColumn = GetUnknownBoatsInColumn(column).Count;
                var remainingBoatsToFind = ColumnCount(column) - numberOfBoatsInColumn;

                if (remainingBoatsToFind > 0 && remainingBoatsToFind == unknownCellsInColumn.Count)
                {
                    foreach (var row in unknownCellsInColumn)
                    {
                        _state[column, row] = CellType.UnknownBoatPart;
                    }

                    return new Solution { Description = "All remaining cells in this column contain boats" };
                }

                if (remainingBoatsToFind == 0 && unknownCellsInColumn.Count > 0)
                {
                    foreach (var row in unknownCellsInColumn)
                    {
                        _state[column, row] = CellType.Water;
                    }

                    return new Solution { Description = "All remaining cells in this column contain water" };
                }
            }

            return null;
        }

        private List<int> GetUnknownCellsInColumn(int column)
        {
            var results = new List<int>();

            for (int row = 0; row < NumberOfRows; row++)
            {
                if (_state[column, row] == CellType.Unknown)
                {
                    results.Add(row);
                }
            }

            return results;
        }

        private List<int> GetUnknownBoatsInColumn(int column)
        {
            var results = new List<int>();

            for (int row = 0; row < NumberOfRows; row++)
            {
                if (_state[column, row] != CellType.Unknown && _state[column, row] != CellType.Water)
                {
                    results.Add(row);
                }
            }

            return results;
        }
    }
}
