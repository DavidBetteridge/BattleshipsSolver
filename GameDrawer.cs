using System;
using System.Drawing;
using System.Linq;
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
            g.TranslateTransform(100 + (CellSize / 2) - 15, 50);

            for (int column = 0; column < _game.NumberOfColumns; column++)
            {
                g.DrawString(_game.ColumnCount(column).ToString(), _symbolFont, Brushes.Black, column * CellSize, 0);
                g.DrawString(_game.ColumnCount(column).ToString(), _symbolFont, Brushes.Black, column * CellSize, (_game.NumberOfRows + 1) * CellSize);

                DrawChainForColumnInformation(g, column);
            }

            g.ResetTransform();
            g.TranslateTransform(60, 100 + (CellSize / 2) - 20);

            for (int row = 0; row < _game.NumberOfRows; row++)
            {
                g.DrawString(_game.RowCount(row).ToString(), _symbolFont, Brushes.Black, 0, row * CellSize);
                g.DrawString(_game.RowCount(row).ToString(), _symbolFont, Brushes.Black, (_game.NumberOfColumns + 1) * CellSize, row * CellSize);

                DrawChainForRowInformation(g, row);
            }

            g.ResetTransform();
            g.TranslateTransform(100, 100);

            for (int column = 0; column < _game.NumberOfColumns; column++)
            {
                for (int row = 0; row < _game.NumberOfRows; row++)
                {
                    DrawSymbol(g, _game.CellContents(column, row), column, row, Brushes.Black);
                }
            }

            g.DrawRectangle(_borderPen, 0, 0, _game.NumberOfColumns * CellSize, _game.NumberOfRows * CellSize);

            for (int column = 1; column < _game.NumberOfColumns; column++)
            {
                g.DrawLine(Pens.Black, column * CellSize, 0, column * CellSize, _game.NumberOfRows * CellSize);
            }

            for (int row = 1; row < _game.NumberOfRows; row++)
            {
                g.DrawLine(Pens.Black, 0, row * CellSize, _game.NumberOfColumns * CellSize, row * CellSize);
            }
        }

        private void DrawChainForRowInformation(Graphics g, int row)
        {
            var longestedChain = _game.FindChainsForRow(row).Where(chain => !chain.IsCompleted).OrderByDescending(chain => chain.Length).FirstOrDefault()?.Length ?? 0;
            longestedChain = Math.Min(longestedChain, _game.RowCount(row));
            g.DrawString(longestedChain.ToString(), _symbolFont, Brushes.Red, (_game.NumberOfColumns + 2) * CellSize, row * CellSize);

            var c = _game.FindChainsForRow(row).Where(chain => chain.IsCompleted).Select(chain => chain.Length);
            var boats = string.Join("/", c);
            g.DrawString(boats, _symbolFont, Brushes.Green, (_game.NumberOfColumns + 3) * CellSize, row * CellSize);
        }

        private void DrawChainForColumnInformation(Graphics g, int column)
        {
            var longestedChain = _game.FindChainsForColumn(column).Where(chain => !chain.IsCompleted).OrderByDescending(chain => chain.Length).FirstOrDefault()?.Length ?? 0;
            longestedChain = Math.Min(longestedChain, _game.ColumnCount(column));
            g.DrawString(longestedChain.ToString(), _symbolFont, Brushes.Red, column * CellSize, (_game.NumberOfRows + 2) * CellSize);

            var boats = string.Join("/", _game.FindChainsForColumn(column).Where(chain => chain.IsCompleted).Select(chain => chain.Length));
            g.DrawString(boats, _symbolFont, Brushes.Green, column * CellSize, (_game.NumberOfRows + 3) * CellSize);
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
                    g.FillRectangle(Brushes.DarkBlue, x, y, CellSize, CellSize);
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
