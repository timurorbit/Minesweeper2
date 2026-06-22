using System;
using System.Collections.Generic;

namespace Minesweeper.Domain
{
    /// <summary>The gameplay service the presentation layer drives and observes.</summary>
    public interface IGameSession
    {
        IBoard Board { get; }
        GameStatus Status { get; }
        float ElapsedSeconds { get; }
        bool IsPaused { get; }

        event Action<IReadOnlyList<Coordinate>> CellsChanged;
        event Action<float> TimeChanged;
        event Action<GameStatus> Ended;
        event Action Restarted;

        void Reveal(Coordinate coordinate);
        void ToggleFlag(Coordinate coordinate);
        void Pause();
        void Resume();
        void Restart();
        void Tick(float deltaSeconds);
    }
}
