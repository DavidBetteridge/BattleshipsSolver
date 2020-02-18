using System.Linq;
using System.Threading.Tasks;
using static BattleshipSolver.Game;

namespace BattleshipSolver
{
    public class LoadGameFromFile
    {
        public async Task<Game> Load(string filename)
        {
            var lines = await System.IO.File.ReadAllLinesAsync(filename).ConfigureAwait(false);
            var numberOfColumns = lines[0].Length - 1;
            var numberOfRows = lines.Length - 1;

            var columnCounts = lines[0].Skip(1).Select(x => int.Parse(x.ToString())).ToArray();
            var rowCounts = lines.Skip(1).Select(l => int.Parse(l[0].ToString())).ToArray();

            var initialState = new CellType[numberOfColumns, numberOfRows];

            for (int column = 0; column < numberOfColumns; column++)
            {
                for (int row = 0; row < numberOfRows; row++)
                {
                    initialState[column, row] = lines[row + 1][column + 1] switch
                    {
                        'M' => CellType.Water,
                        'S' => CellType.SouthEnd,
                        'N' => CellType.NorthEnd,
                        'W' => CellType.WestEnd,
                        'E' => CellType.EastEnd,
                        'R' => CellType.Round,
                        '?' => CellType.UnknownBoatPart,
                        _ => CellType.Unknown,
                    };
                }
            }

            return new Game(columnCounts, rowCounts, initialState);
        }
    }
}
