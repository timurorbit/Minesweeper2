using Minesweeper.Domain;
using NUnit.Framework;

namespace Minesweeper.Tests
{
    public sealed class BoardTests
    {
        // 3x3 grid with a single mine at (0,0).
        private static Board BoardWithMineAtOrigin()
            => new Board(3, 3, 1, new FixedMinePlacer(new Coordinate(0, 0)));

        [Test]
        public void AdjacentMines_AreCountedAroundTheMine()
        {
            var board = BoardWithMineAtOrigin();
            board.Reveal(new Coordinate(2, 2));

            Assert.AreEqual(1, board.CellAt(new Coordinate(1, 0)).AdjacentMines);
            Assert.AreEqual(1, board.CellAt(new Coordinate(0, 1)).AdjacentMines);
            Assert.AreEqual(1, board.CellAt(new Coordinate(1, 1)).AdjacentMines);
            Assert.AreEqual(0, board.CellAt(new Coordinate(2, 2)).AdjacentMines);
        }

        [Test]
        public void RevealingNumberedCell_RevealsOnlyThatCell()
        {
            var board = BoardWithMineAtOrigin();
            var result = board.Reveal(new Coordinate(1, 0));

            Assert.AreEqual(1, result.Revealed.Count);
            Assert.AreEqual(CellState.Revealed, board.CellAt(new Coordinate(1, 0)).State);
        }

        [Test]
        public void RevealingEmptyCell_FloodsAcrossZeroAdjacentRegion()
        {
            var board = BoardWithMineAtOrigin();
            var result = board.Reveal(new Coordinate(2, 2));

            Assert.AreEqual(8, result.Revealed.Count);
        }

        [Test]
        public void RevealingMine_ReportsHitMine()
        {
            var board = BoardWithMineAtOrigin();
            var result = board.Reveal(new Coordinate(0, 0));
            Assert.IsTrue(result.HitMine);
        }

        [Test]
        public void RevealingAllSafeCells_ReportsCleared()
        {
            var board = BoardWithMineAtOrigin();
            var result = board.Reveal(new Coordinate(2, 2));
            Assert.IsTrue(result.Cleared);
        }

        [Test]
        public void FirstReveal_IsAlwaysSafe()
        {
            var first = new Coordinate(4, 4);
            for (int seed = 0; seed < 50; seed++)
            {
                var board = new Board(9, 9, 80, new RandomMinePlacer(new SystemRandom(seed)));
                var result = board.Reveal(first);
                Assert.IsFalse(result.HitMine, $"seed {seed} hit a mine on first click");
                Assert.IsFalse(board.CellAt(first).IsMine, $"seed {seed} put a mine under first click");
            }
        }

        [Test]
        public void FlaggedCell_CannotBeRevealed()
        {
            var board = BoardWithMineAtOrigin();
            var target = new Coordinate(1, 0);
            board.ToggleFlag(target);

            var result = board.Reveal(target);
            Assert.AreEqual(0, result.Revealed.Count);
            Assert.AreEqual(CellState.Flagged, board.CellAt(target).State);
        }

        [Test]
        public void ToggleFlag_TogglesHiddenAndFlagged()
        {
            var board = BoardWithMineAtOrigin();
            var c = new Coordinate(2, 2);

            board.ToggleFlag(c);
            Assert.AreEqual(CellState.Flagged, board.CellAt(c).State);
            board.ToggleFlag(c);
            Assert.AreEqual(CellState.Hidden, board.CellAt(c).State);
        }
    }
}
