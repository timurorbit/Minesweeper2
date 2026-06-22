using System;
using System.Collections.Generic;

namespace Minesweeper.Domain
{
    /// <summary>
    /// Orchestrates one playthrough over a <see cref="Board"/>: owns status + timer, exposes
    /// reveal/flag/pause/restart, and raises events the presentation layer subscribes to.
    /// Win = every mine flagged and every safe cell revealed; loss = a revealed mine.
    /// Pure C# (no SignalBus) — the timer is advanced from Unity via <see cref="Tick"/>.
    /// </summary>
    public sealed class GameSession
    {
        private readonly Board board;

        private bool hasStarted;
        private bool timerRunning;

        public Board Board => board;
        public GameStatus Status { get; private set; } = GameStatus.InProgress;
        public float ElapsedSeconds { get; private set; }
        public bool IsPaused { get; private set; }

        public event Action<IReadOnlyList<Coordinate>> CellsChanged;
        public event Action<float> TimeChanged;
        public event Action<GameStatus> Ended;
        public event Action Restarted;

        public GameSession(Board board) => this.board = board;

        public void Reveal(Coordinate c)
        {
            if (IsPaused || Status != GameStatus.InProgress)
                return;

            if (!hasStarted)
            {
                hasStarted = true;
                timerRunning = true;
            }

            var result = board.Reveal(c);
            if (result.Revealed.Count > 0)
                CellsChanged?.Invoke(result.Revealed);

            if (result.HitMine)
                End(GameStatus.Lost);
            else if (board.IsSolved())
                End(GameStatus.Won);
        }

        public void ToggleFlag(Coordinate c)
        {
            if (IsPaused || Status != GameStatus.InProgress)
                return;

            board.ToggleFlag(c);
            CellsChanged?.Invoke(new[] { c });
        }

        public void Pause()
        {
            if (Status != GameStatus.InProgress)
                return;
            IsPaused = true;
            timerRunning = false;
        }

        public void Resume()
        {
            if (Status != GameStatus.InProgress)
                return;
            IsPaused = false;
            if (hasStarted)
                timerRunning = true;
        }

        public void Restart()
        {
            board.Reset();
            Status = GameStatus.InProgress;
            ElapsedSeconds = 0f;
            hasStarted = false;
            timerRunning = false;
            IsPaused = false;
            Restarted?.Invoke();
        }

        public void Tick(float deltaSeconds)
        {
            if (!timerRunning)
                return;
            ElapsedSeconds += deltaSeconds;
            TimeChanged?.Invoke(ElapsedSeconds);
        }

        private void End(GameStatus status)
        {
            Status = status;
            timerRunning = false;
            Ended?.Invoke(status);
        }
    }
}
