namespace Minesweeper.Domain
{
    public enum CellState { Hidden, Revealed, Flagged }

    public sealed class Cell
    {
        public bool IsMine { get; internal set; }
        public int AdjacentMines { get; internal set; }
        public CellState State { get; internal set; } = CellState.Hidden;
    }
}
