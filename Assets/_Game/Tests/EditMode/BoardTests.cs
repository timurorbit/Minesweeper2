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
            board.Reveal(new Coordinate(2, 2)); // first reveal triggers placement + adjacency

            Assert.AreEqual(1, board.CellAt(new Coordinate(1, 0)).AdjacentMines);
            Assert.AreEqual(1, board.CellAt(new Coordinate(0, 1)).AdjacentMines);
            Assert.AreEqual(1, board.CellAt(new Coordinate(1, 1)).AdjacentMines);
            Assert.AreEqual(0, board.CellAt(new Coordinate(2, 2)).AdjacentMines);
        }

        [Test]
        public void RevealingNumberedCell_RevealsOnlyThatCell()
        {
            var board = BoardWithMineAtOrigin();
            var revealed = board.Reveal(new Coordinate(1, 0)); // touches the mine -> "1"

            Assert.AreEqual(1, revealed.Count);
            Assert.AreEqual(CellState.Revealed, board.CellAt(new Coordinate(1, 0)).State);
        }

        [Test]
        public void RevealingEmptyCell_FloodsAcrossZeroAdjacentRegion()
        {
            var board = BoardWithMineAtOrigin();
            var revealed = board.Reveal(new Coordinate(2, 2)); // empty corner

            Assert.AreEqual(8, revealed.Count); // every cell except the mine
        }

        [Test]
        public void RevealingMine_LosesTheGame()
        {
            var board = BoardWithMineAtOrigin();
            board.Reveal(new Coordinate(0, 0));
            Assert.AreEqual(GameStatus.Lost, board.Status);
        }

        [Test]
        public void RevealingAllSafeCells_WinsTheGame()
        {
            var board = BoardWithMineAtOrigin();
            board.Reveal(new Coordinate(2, 2)); // floods all 8 safe cells
            Assert.AreEqual(GameStatus.Won, board.Status);
        }

        [Test]
        public void FirstReveal_IsAlwaysSafe()
        {
            var first = new Coordinate(4, 4);
            for (int seed = 0; seed < 50; seed++)
            {
                var board = new Board(9, 9, 80, new RandomMinePlacer(new SystemRandom(seed)));
                board.Reveal(first);
                Assert.AreNotEqual(GameStatus.Lost, board.Status, $"seed {seed} lost on first click");
                Assert.IsFalse(board.CellAt(first).IsMine, $"seed {seed} put a mine under first click");
            }
        }

        [Test]
        public void FlaggedCell_CannotBeRevealed()
        {
            var board = BoardWithMineAtOrigin();
            var target = new Coordinate(1, 0);
            board.ToggleFlag(target);

            var revealed = board.Reveal(target);
            Assert.AreEqual(0, revealed.Count);
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
