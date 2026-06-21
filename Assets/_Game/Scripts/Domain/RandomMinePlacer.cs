using System;
using System.Collections.Generic;

namespace Minesweeper.Domain
{
    public sealed class RandomMinePlacer : IMinePlacer
    {
        private readonly IRandom random;

        public RandomMinePlacer(IRandom random) => this.random = random;

        public IReadOnlyCollection<Coordinate> Place(int width, int height, int mineCount, Coordinate safe)
        {
            var candidates = new List<Coordinate>(width * height);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var c = new Coordinate(x, y);
                if (!c.Equals(safe))
                    candidates.Add(c);
            }

            mineCount = Math.Min(mineCount, candidates.Count);

            // Partial Fisher-Yates: shuffle the first mineCount slots and take them.
            var mines = new HashSet<Coordinate>(mineCount);
            for (int i = 0; i < mineCount; i++)
            {
                int j = i + random.Next(candidates.Count - i);
                (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
                mines.Add(candidates[i]);
            }
            return mines;
        }
    }
}
