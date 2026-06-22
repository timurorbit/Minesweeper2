using Minesweeper.Domain;
using Minesweeper.Presentation;
using Minesweeper.Presentation.Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameConfig config;
    [SerializeField] private BoardView boardView;
    [SerializeField] private HudView hudView;
    [SerializeField] private GameInput gameInput;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(config);
        builder.Register<IRandom>(_ => new SystemRandom(), Lifetime.Singleton);
        builder.Register<IMinePlacer, RandomMinePlacer>(Lifetime.Singleton);
        builder.Register(c =>
        {
            var cfg = c.Resolve<GameConfig>();
            return new Board(cfg.Width, cfg.Height, cfg.MineCount, c.Resolve<IMinePlacer>());
        }, Lifetime.Singleton);
        builder.Register<GameSession>(Lifetime.Singleton);

        builder.RegisterComponent(boardView);
        builder.RegisterComponent(hudView);
        builder.RegisterComponent(gameInput);

        builder.RegisterEntryPoint<GamePresenter>();
    }
}
