using Minesweeper.Domain;
using Minesweeper.Presentation.Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Minesweeper.Presentation
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private BoardView boardView;
        [SerializeField] private HudView hudView;
        [SerializeField] private GameInput gameInput;
        [SerializeField] private MainMenuView mainMenuView;
        [SerializeField] private PauseView pauseView;
        [SerializeField] private GameOverView gameOverView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(config);
            builder.Register<IRandom>(_ => new SystemRandom(), Lifetime.Singleton);
            builder.Register<IMinePlacer, RandomMinePlacer>(Lifetime.Singleton);
            builder.Register(c =>
            {
                var placer = c.Resolve<IMinePlacer>();
                return new GameSession(() => new Board(config.Width, config.Height, config.MineCount, placer));
            }, Lifetime.Singleton);

            builder.RegisterComponent(boardView);
            builder.RegisterComponent(hudView);
            builder.RegisterComponent(gameInput);
            builder.RegisterComponent(mainMenuView);
            builder.RegisterComponent(pauseView);
            builder.RegisterComponent(gameOverView);

            builder.RegisterEntryPoint<GamePresenter>();
            builder.RegisterEntryPoint<GameFlow>();
        }
    }
}
