using System;
using System.Collections.Generic;
using Minesweeper.Domain;
using UnityEngine;
using VContainer.Unity;

namespace Minesweeper.Presentation
{
    public sealed class GamePresenter : IStartable, ITickable, IDisposable
    {
        private readonly GameSession session;
        private readonly BoardView boardView;
        private readonly HudView hudView;
        private readonly GameInput input;

        public GamePresenter(GameSession session, BoardView boardView, HudView hudView, GameInput input)
        {
            this.session = session;
            this.boardView = boardView;
            this.hudView = hudView;
            this.input = input;
        }

        public void Start()
        {
            boardView.Build(session.Board);
            boardView.CellLeftClicked += session.Reveal;
            boardView.CellRightClicked += session.ToggleFlag;
            input.RestartRequested += session.Restart;

            session.CellsChanged += OnCellsChanged;
            session.TimeChanged += hudView.SetTime;
            session.Ended += OnEnded;
            session.Restarted += OnRestarted;

            hudView.Clear();
        }

        public void Tick() => session.Tick(Time.deltaTime);

        public void Dispose()
        {
            boardView.CellLeftClicked -= session.Reveal;
            boardView.CellRightClicked -= session.ToggleFlag;
            input.RestartRequested -= session.Restart;

            session.CellsChanged -= OnCellsChanged;
            session.TimeChanged -= hudView.SetTime;
            session.Ended -= OnEnded;
            session.Restarted -= OnRestarted;
        }

        private void OnCellsChanged(IReadOnlyList<Coordinate> changed)
        {
            for (int i = 0; i < changed.Count; i++)
                boardView.RenderCell(changed[i]);
        }

        private void OnEnded(GameStatus status) => hudView.SetStatus(status == GameStatus.Won ? "You win!" : "Game over");

        private void OnRestarted()
        {
            boardView.RenderAll();
            hudView.Clear();
        }
    }
}
