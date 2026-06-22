using System.Collections.Generic;

namespace Minesweeper.Domain
{
    /// <summary>Outcome of a single <see cref="Board.Reveal"/>: the cells opened and whether a mine was hit.</summary>
    public readonly struct RevealResult
    {
        public IReadOnlyList<Coordinate> Revealed { get; }
        public bool HitMine { get; }

        public RevealResult(IReadOnlyList<Coordinate> revealed, bool hitMine)
        {
            Revealed = revealed;
            HitMine = hitMine;
        }
    }
}
