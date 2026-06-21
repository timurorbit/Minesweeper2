using System.Linq;
using Minesweeper.Domain;
using NUnit.Framework;

namespace Minesweeper.Tests
{
    public sealed class MinePlacerTests
    {
        [Test]
        public void Place_ReturnsRequestedMineCount()
        {
            var mines = new RandomMinePlacer(new SystemRandom(1)).Place(9, 9, 10, new Coordinate(0, 0));
            Assert.AreEqual(10, mines.Count);
        }

        [Test]
        public void Place_NeverPutsMineOnSafeCell()
        {
            var safe = new Coordinate(4, 4);
            for (int seed = 0; seed < 50; seed++)
            {
                var mines = new RandomMinePlacer(new SystemRandom(seed)).Place(9, 9, 80, safe);
                Assert.IsFalse(mines.Contains(safe), $"seed {seed} placed a mine on the safe cell");
            }
        }

        [Test]
        public void Place_WithSameSeed_IsDeterministic()
        {
            var a = new RandomMinePlacer(new SystemRandom(42)).Place(9, 9, 10, new Coordinate(0, 0));
            var b = new RandomMinePlacer(new SystemRandom(42)).Place(9, 9, 10, new Coordinate(0, 0));
            CollectionAssert.AreEquivalent(a, b);
        }
    }
}
