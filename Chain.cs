namespace BattleshipSolver
{
    public class Chain
    {
        public CellLocation Start { get; set; }
        public CellLocation End { get; set; }
        public int Length { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsVertical { get; set; }
    }
}
