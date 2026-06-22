using System;
using Minesweeper.Domain;
using VContainer.Unity;

namespace Minesweeper.Presentation
{
    public enum GameState { MainMenu, Playing, Paused, GameOver }

    public sealed class GameFlow : IStartable, IDisposable
    {
        private readonly GameSession session;
        private readonly MainMenuView mainMenu;
        private readonly PauseView pause;
        private readonly GameOverView gameOver;
        private readonly HudView hud;
        private readonly GameInput input;

        private GameState state;

        public GameFlow(GameSession session, MainMenuView mainMenu, PauseView pause,
                        GameOverView gameOver, HudView hud, GameInput input)
        {
            this.session = session;
            this.mainMenu = mainMenu;
            this.pause = pause;
            this.gameOver = gameOver;
            this.hud = hud;
            this.input = input;
        }

        public void Start()
        {
            mainMenu.StartClicked += StartGame;
            hud.PauseClicked += PauseGame;
            pause.ResumeClicked += ResumeGame;
            pause.RestartClicked += RestartGame;
            pause.MenuClicked += GoToMenu;
            gameOver.RestartClicked += RestartGame;
            gameOver.MenuClicked += GoToMenu;
            session.Ended += OnEnded;
            input.RestartRequested += OnRestartKey;

            SetState(GameState.MainMenu);
        }

        public void Dispose()
        {
            mainMenu.StartClicked -= StartGame;
            hud.PauseClicked -= PauseGame;
            pause.ResumeClicked -= ResumeGame;
            pause.RestartClicked -= RestartGame;
            pause.MenuClicked -= GoToMenu;
            gameOver.RestartClicked -= RestartGame;
            gameOver.MenuClicked -= GoToMenu;
            session.Ended -= OnEnded;
            input.RestartRequested -= OnRestartKey;
        }

        private void StartGame()
        {
            session.Restart();
            SetState(GameState.Playing);
        }

        private void PauseGame()
        {
            if (state != GameState.Playing)
                return;
            session.Pause();
            SetState(GameState.Paused);
        }

        private void ResumeGame()
        {
            session.Resume();
            SetState(GameState.Playing);
        }

        private void RestartGame()
        {
            session.Restart();
            SetState(GameState.Playing);
        }

        private void GoToMenu() => SetState(GameState.MainMenu);

        private void OnEnded(GameStatus status)
        {
            gameOver.SetResult(status);
            SetState(GameState.GameOver);
        }

        private void OnRestartKey()
        {
            if (state != GameState.MainMenu)
                RestartGame();
        }

        private void SetState(GameState next)
        {
            state = next;
            mainMenu.SetVisible(next == GameState.MainMenu);
            pause.SetVisible(next == GameState.Paused);
            gameOver.SetVisible(next == GameState.GameOver);
        }
    }
}
