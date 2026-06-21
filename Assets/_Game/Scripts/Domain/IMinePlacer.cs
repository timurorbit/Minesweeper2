using System.Collections.Generic;

namespace Minesweeper.Domain
{
    /// <summary>Decides where mines go, always leaving the first-clicked cell mine-free.</summary>
    public interface IMinePlacer
    {
        IReadOnlyCollection<Coordinate> Place(int width, int height, int mineCount, Coordinate safe);
    }
}
