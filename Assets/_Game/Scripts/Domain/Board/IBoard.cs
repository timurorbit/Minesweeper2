namespace Minesweeper.Domain
{
    /// <summary>Read-only view of the board for the presentation layer.</summary>
    public interface IBoard
    {
        int Width { get; }
        int Height { get; }
        Cell CellAt(Coordinate coordinate);
    }
}
