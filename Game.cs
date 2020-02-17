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

        internal object Solve()
        {
            return 1;
        }
    }
}
