using System.Collections.Generic;
using Minesweeper.Domain;

namespace Minesweeper.Tests
{
    /// <summary>Test placer with a fixed mine layout (ignores safe + count) for deterministic boards.</summary>
    public sealed class FixedMinePlacer : IMinePlacer
    {
        private readonly Coordinate[] mines;

        public FixedMinePlacer(params Coordinate[] mines) => this.mines = mines;

        public IReadOnlyCollection<Coordinate> Place(int width, int height, int mineCount, Coordinate safe)
            => mines;
    }
}
