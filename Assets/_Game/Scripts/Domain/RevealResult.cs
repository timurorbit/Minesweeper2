using System.Collections.Generic;

namespace Minesweeper.Domain
{
    /// <summary>Outcome of a single <see cref="Board.Reveal"/>: the cells opened and what they mean.</summary>
    public readonly struct RevealResult
    {
        public IReadOnlyList<Coordinate> Revealed { get; }
        public bool HitMine { get; }
        public bool Cleared { get; }

        public RevealResult(IReadOnlyList<Coordinate> revealed, bool hitMine, bool cleared)
        {
            Revealed = revealed;
            HitMine = hitMine;
            Cleared = cleared;
        }
    }
}
