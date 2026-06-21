using Minesweeper.Domain;
using NUnit.Framework;

namespace Minesweeper.Tests
{
    public sealed class GameSessionTests
    {
        // 3x3 session with a single mine at (0,0).
        private static GameSession Session()
            => new GameSession(new Board(3, 3, 1, new FixedMinePlacer(new Coordinate(0, 0))));

        [Test]
        public void Timer_DoesNotRunBeforeFirstReveal()
        {
            var session = Session();
            session.Tick(1f);
            Assert.AreEqual(0f, session.ElapsedSeconds);
        }

        [Test]
        public void Timer_StartsOnFirstReveal()
        {
            var session = Session();
            session.Reveal(new Coordinate(1, 0));
            session.Tick(1.5f);
            Assert.AreEqual(1.5f, session.ElapsedSeconds, 0.0001f);
        }

        [Test]
        public void Pause_StopsTimer_ResumeContinues()
        {
            var session = Session();
            session.Reveal(new Coordinate(1, 0));
            session.Tick(1f);

            session.Pause();
            session.Tick(5f);
            Assert.AreEqual(1f, session.ElapsedSeconds, 0.0001f);

            session.Resume();
            session.Tick(2f);
            Assert.AreEqual(3f, session.ElapsedSeconds, 0.0001f);
        }

        [Test]
        public void Reveal_IsIgnored_WhilePaused()
        {
            var session = Session();
            session.Reveal(new Coordinate(1, 0));
            session.Pause();

            var c = new Coordinate(2, 2);
            session.Reveal(c);
            Assert.AreEqual(CellState.Hidden, session.Board.CellAt(c).State);
        }

        [Test]
        public void RevealingMine_RaisesEndedLost_AndStopsTimer()
        {
            var session = Session();
            GameStatus? ended = null;
            session.Ended += s => ended = s;

            session.Reveal(new Coordinate(0, 0));
            Assert.AreEqual(GameStatus.Lost, ended);

            session.Tick(3f);
            Assert.AreEqual(0f, session.ElapsedSeconds);
        }

        [Test]
        public void RevealingAllSafeCells_RaisesEndedWon()
        {
            var session = Session();
            GameStatus? ended = null;
            session.Ended += s => ended = s;

            session.Reveal(new Coordinate(2, 2));
            Assert.AreEqual(GameStatus.Won, ended);
        }

        [Test]
        public void Reveal_RaisesCellsChanged()
        {
            var session = Session();
            int changed = 0;
            session.CellsChanged += cells => changed += cells.Count;

            session.Reveal(new Coordinate(1, 0));
            Assert.AreEqual(1, changed);
        }

        [Test]
        public void ToggleFlag_FlagsCell_AndRaisesCellsChanged()
        {
            var session = Session();
            bool raised = false;
            session.CellsChanged += _ => raised = true;

            var c = new Coordinate(2, 2);
            session.ToggleFlag(c);

            Assert.AreEqual(CellState.Flagged, session.Board.CellAt(c).State);
            Assert.IsTrue(raised);
        }

        [Test]
        public void Restart_ResetsBoardTimerAndState()
        {
            var session = Session();
            session.Reveal(new Coordinate(1, 0));
            session.Tick(2f);
            Assert.AreEqual(2f, session.ElapsedSeconds, 0.0001f);

            bool restarted = false;
            session.Restarted += () => restarted = true;
            session.Restart();

            Assert.IsTrue(restarted);
            Assert.AreEqual(GameStatus.InProgress, session.Status);
            Assert.AreEqual(0f, session.ElapsedSeconds);
            Assert.AreEqual(CellState.Hidden, session.Board.CellAt(new Coordinate(1, 0)).State);
        }
    }
}
