using Minesweeper.Domain;
using NUnit.Framework;

namespace Minesweeper.Tests
{
    /// <summary>
    /// Phase-0 smoke test: proves the EditMode test pipeline compiles against the
    /// engine-free Domain assembly and actually runs.
    /// </summary>
    public sealed class CoordinateTests
    {
        [Test]
        public void Coordinates_WithSameXY_AreEqual()
        {
            Assert.AreEqual(new Coordinate(3, 5), new Coordinate(3, 5));
        }

        [Test]
        public void Coordinates_WithDifferentXY_AreNotEqual()
        {
            Assert.AreNotEqual(new Coordinate(1, 2), new Coordinate(2, 1));
        }
    }
}
