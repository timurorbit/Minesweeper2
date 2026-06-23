# Minesweeper (Unity)

A Minesweeper clone built in Unity as a test task — a clean, layered, test-driven take on the classic game.

![Minesweeper](Assets/_Game/Screenshot.png)

## Running

- **Unity 6000.3.18f1** (Unity 6.3) — open the project via Unity Hub.
- This repo uses **Git LFS** for image assets — install `git-lfs` before cloning, or the sprites won't pull.
- Open the scene in `Assets/Scenes/` and press **Play** (boots to the main menu).

## Controls

| Action | Input |
|---|---|
| Reveal a cell | **Left click** |
| Flag / unflag a cell | **Right click** |
| Restart | **R** (any time, including after game over) |
| Pause | in-game **Pause** button → Resume / Restart / Exit |

- **Win** by revealing every safe cell (only mines left unopened).
- **Lose** by revealing a mine — all mines are then shown, the clicked one highlighted.
- The **first click is always safe** — mines are placed *after* it.

## Configuration

Board size and mine count are **data-driven** via a `GameConfig` ScriptableObject — no code or scene edits required.

- Asset: `Assets/_Game/Config/GameConfig.asset`
- Fields: **Width**, **Height** (2–100) and **Mine Count** (auto-clamped below the cell count so the first click can stay safe).
- The board is rebuilt from this config on **every new game**, so you can change the values and just press **R**.

To change difficulty, select the `GameConfig` asset and edit the three fields in the Inspector.

## Architecture

The project is split into **independent modules** with a strict, one-way dependency (presentation → domain, never back):

- **`Minesweeper.Domain`** — pure C# game logic: `Board`, `Cell`, mine placement, flood-fill, win/lose, `GameSession`. **Engine-free** (`noEngineReferences`) — it cannot reference `UnityEngine`, which keeps it fully unit-testable.
- **Presentation** (`Minesweeper.Presentation` namespace) — the Unity **MVP** layer: passive **Views** (MonoBehaviours), **Presenters** (`GamePresenter`, `GameFlow`) that mediate, talking to the domain only through **C# events**.
- **`Minesweeper.Tests`** — EditMode unit tests, referencing only the domain (can't reach the presentation layer).

### Highlights

- **MVP** — the model (`GameSession`) raises events; presenters drive passive views; the model has no knowledge of the view.
- **Dependency Injection** (VContainer) — one composition root (`GameLifetimeScope`) wires the whole object graph via constructor injection.
- **Programmed to interfaces** — `IGameSession`, `IBoard`, `IMinePlacer`, `IRandom`. The seedable `IRandom` is what makes mine placement deterministic in tests.
- **Testing** — the engine-free domain is covered by fast EditMode unit tests: mine count, **first-click safety across many seeds**, flood-fill cascade, win/lose detection.
- **Sprite atlas** — all cell/UI sprites are packed into a single Sprite Atlas, so the grid draws in roughly **one batch**.
- **UI performance** — a separate canvas per update-frequency (grid / HUD / screens) so the per-second timer never rebatches the board; only **changed cells** re-render; cells are **pooled** and reused.

### Deliberately not used

- **ECS / DOTS** — overkill for a static grid; the gameplay is event-driven, not data-parallel.
- **Addressables / Asset Bundles** — no large or streamed assets to justify the complexity.

Both are understood — they're simply not the right tool at this scope.

## Tech stack

- Unity 6.3, C#
- [VContainer](https://github.com/hadashiA/VContainer) — dependency injection
- TextMeshPro, Unity UI (UGUI), the Input System
- [Kenney](https://kenney.nl) board-game sprites (CC0)

## Project structure

```
Assets/_Game/
  Scripts/
    Domain/              # engine-free game logic (asmdef: Minesweeper.Domain)
      Board/             #   Board, Cell, Coordinate, RevealResult, IBoard
      Mines/             #   IMinePlacer, RandomMinePlacer
      Randomization/     #   IRandom, SystemRandom
      Session/           #   GameSession, GameStatus, IGameSession
    Presentation/        # Unity MVP layer
      Board/ Hud/ Input/ Screens/ Flow/ Installers/ Config/
    Tests/EditMode/      # unit tests (asmdef: Minesweeper.Tests)
  Config/                # GameConfig + CellSpriteSet assets
  Prefabs/               # cell prefab
```

## Possible improvements

- **Stricter view / presentation split** — hand views render-only data (a view-model / DTO) instead of letting `BoardView` read the domain `Cell` directly.
- **Reactive binding** — replace the manual event→view wiring with **R3 / UniRx** observables for a true MVVM data-binding flow.
- **Addressables for sprites** — if the art set grows, stream the atlas via Addressables rather than bundling it.
- **Shared view pooling** — cells are already pooled within a board; a pool shared across board-size changes / scenes would cut instantiation further.
- **Enforce the presentation module** — it's currently a logical module (folder + namespace); an assembly definition would make the boundary compiler-enforced.
