using System;
using System.Collections.Generic;
using System.Linq;

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
            UnknownBoatPart = 9,
            UnknownHorizontalBoatPart = 10,
            UnknownVerticalBoatPart = 11
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
            var solution = IdentifyBoats();

            if (solution is null)
                solution = FindDeadSquares();

            if (solution is null)
                solution = SolveColumnsUsingTheColumnCounts();

            if (solution is null)
                solution = SolveRowsUsingTheRowCounts();

            if (solution is null)
                solution = GrowBoats();

            return solution;
        }

        private Solution FindDeadSquares()
        {
            bool SetToWater(int column, int row)
            {
                if (column >= 0 && row >= 0 && column < NumberOfColumns && row < NumberOfRows && _state[column, row] != CellType.Water)
                {
                    _state[column, row] = CellType.Water;
                    return true;
                }
                return false;
            }

            for (int row = 0; row < NumberOfRows; row++)
            {
                for (int column = 0; column < NumberOfColumns; column++)
                {
                    var set = false;
                    switch (_state[column, row])
                    {
                        case CellType.SouthEnd:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column - 1, row);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column, row + 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column + 1, row);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        case CellType.NorthEnd:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column - 1, row);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column, row - 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column + 1, row);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        case CellType.WestEnd:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column, row - 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column - 1, row);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column, row + 1);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        case CellType.EastEnd:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column, row - 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column + 1, row);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column, row + 1);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        case CellType.VerticalMiddle:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column - 1, row);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column + 1, row);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        case CellType.HorizontalMiddle:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column, row - 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column, row + 1);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        case CellType.Round:
                            set |= SetToWater(column - 1, row - 1);
                            set |= SetToWater(column, row - 1);
                            set |= SetToWater(column + 1, row - 1);
                            set |= SetToWater(column - 1, row);
                            set |= SetToWater(column + 1, row);
                            set |= SetToWater(column - 1, row + 1);
                            set |= SetToWater(column, row + 1);
                            set |= SetToWater(column + 1, row + 1);
                            break;
                        default:
                            break;
                    }

                    if (set)
                        return new Solution { Description = "Boat is surrounded by water" };
                }
            }
            return null;
        }

        private Solution IdentifyBoats()
        {
            for (int row = 0; row < NumberOfRows; row++)
            {
                for (int column = 0; column < NumberOfColumns; column++)
                {
                    if (_state[column, row] == CellType.UnknownBoatPart)
                    {
                        var neighbours = GetNeighbours(column, row);

                        if (neighbours.All(location => _state[location.Column, location.Row] == CellType.Water))
                        {
                            _state[column, row] = CellType.Round;
                            return new Solution { Description = "Must be 1 boat" };
                        }

                        if (row > 0 && _state[column, row - 1] != CellType.Water && _state[column, row - 1] != CellType.Unknown)
                        {
                            _state[column, row] = CellType.UnknownVerticalBoatPart;
                            return new Solution { Description = "Must be vertical boat" };
                        }

                        if (row < (NumberOfRows - 1) && _state[column, row + 1] != CellType.Water && _state[column, row + 1] != CellType.Unknown)
                        {
                            _state[column, row] = CellType.UnknownVerticalBoatPart;
                            return new Solution { Description = "Must be vertical boat" };
                        }

                        if (column > 0 && _state[column-1, row] != CellType.Water && _state[column-1, row] != CellType.Unknown)
                        {
                            _state[column, row] = CellType.UnknownHorizontalBoatPart;
                            return new Solution { Description = "Must be horizontal boat" };
                        }

                        if (column < (NumberOfColumns - 1) && _state[column + 1, row] != CellType.Water && _state[column + 1, row] != CellType.Unknown)
                        {
                            _state[column, row] = CellType.UnknownHorizontalBoatPart;
                            return new Solution { Description = "Must be horizontal boat" };
                        }
                    }

                    if (_state[column, row] == CellType.UnknownVerticalBoatPart)
                    {
                        if (row == 0 || _state[column, row - 1] == CellType.Water)
                        {
                            _state[column, row] = CellType.NorthEnd;
                            return new Solution { Description = "Must be north end" };
                        }
                        if (row == (NumberOfRows - 1) || _state[column, row + 1] == CellType.Water)
                        {
                            _state[column, row] = CellType.SouthEnd;
                            return new Solution { Description = "Must be south end" };
                        }

                        if (row < (NumberOfRows - 1) && IsVertical(_state[column, row + 1]) &&
                           row > 0 && IsVertical(_state[column, row - 1]))
                        {
                            _state[column, row] = CellType.VerticalMiddle;
                            return new Solution { Description = "Must be vertical middle" };
                        }
                    }

                    if (_state[column, row] == CellType.UnknownHorizontalBoatPart)
                    {
                        if (column == 0 || _state[column - 1, row] == CellType.Water)
                        {
                            _state[column, row] = CellType.WestEnd;
                            return new Solution { Description = "Must be west end" };
                        }
                        if (column == (NumberOfColumns - 1) || _state[column + 1, row] == CellType.Water)
                        {
                            _state[column, row] = CellType.EastEnd;
                            return new Solution { Description = "Must be east end" };
                        }

                        if (column < (NumberOfColumns - 1) && IsHorizontal(_state[column + 1, row]) &&
                            column > 0 && IsHorizontal(_state[column - 1, row]))
                        {
                            _state[column, row] = CellType.HorizontalMiddle;
                            return new Solution { Description = "Must be horizontal middle" };
                        }
                    }

                }
            }
            return null;
        }

        private bool IsVertical(CellType cellType)
        {
            return cellType switch
            {
                CellType.UnknownVerticalBoatPart => true,
                CellType.SouthEnd => true,
                CellType.NorthEnd => true,
                CellType.VerticalMiddle => true,
                _ => false,
            };
        }

        private bool IsHorizontal(CellType cellType)
        {
            return cellType switch
            {
                CellType.UnknownHorizontalBoatPart => true,
                CellType.WestEnd => true,
                CellType.EastEnd => true,
                CellType.HorizontalMiddle => true,
                _ => false,
            };
        }

        private Solution GrowBoats()
        {
            for (int row = 0; row < NumberOfRows; row++)
            {
                for (int column = 0; column < NumberOfColumns; column++)
                {
                    switch (_state[column, row])
                    {
                        case CellType.SouthEnd:
                            if (_state[column, row - 1] == CellType.Unknown)
                            {
                                _state[column, row - 1] = CellType.UnknownVerticalBoatPart;
                                return new Solution { Description = "Growing southend of boat" };
                            }
                            break;
                        case CellType.NorthEnd:
                            if (_state[column, row + 1] == CellType.Unknown)
                            {
                                _state[column, row + 1] = CellType.UnknownVerticalBoatPart;
                                return new Solution { Description = "Growing north end of boat" };
                            }
                            break;
                        case CellType.WestEnd:
                            if (_state[column + 1, row] == CellType.Unknown)
                            {
                                _state[column + 1, row] = CellType.UnknownHorizontalBoatPart;
                                return new Solution { Description = "Growing west end of boat" };
                            }
                            break;
                        case CellType.EastEnd:
                            if (_state[column - 1, row] == CellType.Unknown)
                            {
                                _state[column - 1, row] = CellType.UnknownHorizontalBoatPart;
                                return new Solution { Description = "Growing east end of boat" };
                            }
                            break;
                        case CellType.VerticalMiddle:
                            if (_state[column, row - 1] == CellType.Unknown)
                            {
                                _state[column, row - 1] = CellType.UnknownVerticalBoatPart;
                                return new Solution { Description = "Growing middle of boat" };
                            }
                            if (_state[column, row + 1] == CellType.Unknown)
                            {
                                _state[column, row + 1] = CellType.UnknownVerticalBoatPart;
                                return new Solution { Description = "Growing middle of boat" };
                            }
                            break;
                        case CellType.HorizontalMiddle:
                            if (_state[column - 1, row] == CellType.Unknown)
                            {
                                _state[column - 1, row] = CellType.UnknownHorizontalBoatPart;
                                return new Solution { Description = "Growing middle of boat" };
                            }
                            if (_state[column + 1, row] == CellType.Unknown)
                            {
                                _state[column + 1, row] = CellType.UnknownHorizontalBoatPart;
                                return new Solution { Description = "Growing middle of boat" };
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return null;
        }

        private Solution SolveRowsUsingTheRowCounts()
        {
            for (int row = 0; row < NumberOfRows; row++)
            {
                var unknownCellsInRow = GetUnknownCellsInRow(row);
                var numberOfBoatsInRow = GetUnknownBoatsInRow(row).Count;
                var remainingBoatsToFind = RowCount(row) - numberOfBoatsInRow;

                if (remainingBoatsToFind > 0 && remainingBoatsToFind == unknownCellsInRow.Count)
                {
                    foreach (var column in unknownCellsInRow)
                    {
                        _state[column, row] = CellType.UnknownBoatPart;
                    }

                    return new Solution { Description = "All remaining cells in this row contain boats" };
                }

                if (remainingBoatsToFind == 0 && unknownCellsInRow.Count > 0)
                {
                    foreach (var column in unknownCellsInRow)
                    {
                        _state[column, row] = CellType.Water;
                    }

                    return new Solution { Description = "All remaining cells in this row contain water" };
                }
            }

            return null;
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

        private List<int> GetUnknownCellsInRow(int row)
        {
            var results = new List<int>();

            for (int column = 0; column < NumberOfColumns; column++)
            {
                if (_state[column, row] == CellType.Unknown)
                {
                    results.Add(column);
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

        private List<int> GetUnknownBoatsInRow(int row)
        {
            var results = new List<int>();

            for (int column = 0; column < NumberOfColumns; column++)
            {
                if (_state[column, row] != CellType.Unknown && _state[column, row] != CellType.Water)
                {
                    results.Add(column);
                }
            }

            return results;
        }

        private List<CellLocation> GetNeighbours(int column, int row)
        {
            var neighbours = new List<CellLocation>();

            /*
             *   123
             *   4 5
             *   678
             */

            if (column > 0 && row > 0) neighbours.Add(new CellLocation(column - 1, row - 1));
            if (row > 0) neighbours.Add(new CellLocation(column, row - 1));
            if (column < (NumberOfColumns - 1) && row > 0) neighbours.Add(new CellLocation(column + 1, row - 1));

            if (column > 0) neighbours.Add(new CellLocation(column - 1, row));
            if (column < (NumberOfColumns - 1)) neighbours.Add(new CellLocation(column + 1, row));

            if (column > 0 && row < (NumberOfRows - 1)) neighbours.Add(new CellLocation(column - 1, row + 1));
            if (row < (NumberOfRows - 1)) neighbours.Add(new CellLocation(column, row + 1));
            if (column < (NumberOfColumns - 1) && row < (NumberOfRows - 1)) neighbours.Add(new CellLocation(column + 1, row + 1));

            return neighbours;
        }
    }
}
