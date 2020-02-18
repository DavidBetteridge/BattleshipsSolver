using System.Drawing;
using static BattleshipSolver.Game;

namespace BattleshipSolver
{
    class GameDrawer
    {
        private const int CellSize = 50;
        private readonly Font _symbolFont = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private readonly Font _smallFont = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private readonly Font _infoFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private readonly Pen _borderPen = new Pen(Brushes.Black, 3);
        private Game _game;

        public GameDrawer(Game game)
        {
            _game = game;
        }

        public void Draw(Graphics g)
        {
            g.TranslateTransform(100 + (CellSize/2) - 15, 50);

            for (int column = 0; column < _game.NumberOfColumns; column++)
            {
                g.DrawString(_game.ColumnCount(column).ToString(), _symbolFont, Brushes.Black, column * CellSize, 0);
                g.DrawString(_game.ColumnCount(column).ToString(), _symbolFont, Brushes.Black, column * CellSize, (_game.NumberOfRows+1) * CellSize);
            }

            g.ResetTransform();
            g.TranslateTransform(60, 100 + (CellSize / 2) - 20);

            for (int row = 0; row < _game.NumberOfRows; row++)
            {
                g.DrawString(_game.RowCount(row).ToString(), _symbolFont, Brushes.Black, 0, row * CellSize);
                g.DrawString(_game.RowCount(row).ToString(), _symbolFont, Brushes.Black, (_game.NumberOfColumns + 1) * CellSize, row * CellSize);
            }

            g.ResetTransform();
            g.TranslateTransform(100, 100);

            g.DrawRectangle(_borderPen, 0, 0, _game.NumberOfColumns * CellSize, _game.NumberOfRows * CellSize);

            for (int column = 1; column < _game.NumberOfColumns; column++)
            {
                g.DrawLine(Pens.Black, column * CellSize, 0, column * CellSize, _game.NumberOfRows * CellSize);
            }

            for (int row = 1; row < _game.NumberOfRows; row++)
            {
                g.DrawLine(Pens.Black, 0, row * CellSize, _game.NumberOfColumns * CellSize, row * CellSize);
            }

            for (int column = 0; column < _game.NumberOfColumns; column++)
            {
                for (int row = 0; row < _game.NumberOfRows; row++)
                {
                    DrawSymbol(g, _game.CellContents(column, row), column, row, Brushes.Black);
                }
            }
        }

        internal void DrawMove(Graphics g, Solution result)
        {
            //if (!result.CellOfInterest.IsUnused)
            //{
            //    DrawSymbol(g, _game.CellContents(result.CellOfInterest.Column, result.CellOfInterest.Row), result.CellOfInterest.Column, result.CellOfInterest.Row, Brushes.Blue);
            //}

            //foreach (var cell in result.SolvedCells)
            //{
            //    DrawSymbol(g, _game.CellContents(cell.Column, cell.Row), cell.Column, cell.Row, Brushes.Red);
            //}

            g.DrawString(result.Description, _infoFont, Brushes.Black, (_game.NumberOfColumns + 1) * CellSize, 0);

        }

        private void DrawSymbol(Graphics g, CellType symbol, int column, int row, Brush brush)
        {
            var x = column * CellSize;
            var y = row * CellSize;

            switch (symbol)
            {
                case CellType.Unknown:
                    break;
                case CellType.Water:
                    g.FillRectangle(Brushes.Blue, x, y, CellSize, CellSize);
                    break;
                case CellType.SouthEnd:
                    DrawLetter(g, brush, x, y, "S");
                    break;
                case CellType.NorthEnd:
                    DrawLetter(g, brush, x, y, "N"); 
                    break;
                case CellType.WestEnd:
                    DrawLetter(g, brush, x, y, "W"); 
                    break;
                case CellType.EastEnd:
                    DrawLetter(g, brush, x, y, "E"); 
                    break;
                case CellType.VerticalMiddle:
                    DrawLetter(g, brush, x, y, "V"); 
                    break;
                case CellType.HorizontalMiddle:
                    DrawLetter(g, brush, x, y, "H");
                    break;
                case CellType.Round:
                    g.FillEllipse(brush, x + (CellSize / 4), y + (CellSize / 4), CellSize / 2, CellSize / 2);
                    break;
                case CellType.UnknownBoatPart:
                    DrawLetter(g, brush, x, y, "?");
                    break;
                case CellType.UnknownHorizontalBoatPart:
                    DrawLetter(g, brush, x, y, "?H");
                    break;
                case CellType.UnknownVerticalBoatPart:
                    DrawLetter(g, brush, x, y, "?V");
                    break;
                default:
                    break;
            }
        }

        private void DrawLetter(Graphics g, Brush brush, int x, int y, string symbol)
        {
            var symbolSize = g.MeasureString(symbol, _symbolFont, new Point(x, y), StringFormat.GenericDefault);
            g.DrawString(symbol, _symbolFont, brush, x + ((CellSize - symbolSize.Width) / 2), y + ((CellSize - symbolSize.Height) / 2));
        }
    }
}
