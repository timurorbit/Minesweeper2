using System.Collections.Generic;

namespace Minesweeper.Domain
{
    public enum GameStatus { InProgress, Won, Lost }

    public sealed class Board
    {
        private readonly Cell[,] cells;
        private readonly int mineCount;
        private readonly IMinePlacer minePlacer;
        private bool minesPlaced;
        private int safeRevealed;
        private int safeTotal;

        public int Width { get; }
        public int Height { get; }
        public GameStatus Status { get; private set; } = GameStatus.InProgress;

        public Board(int width, int height, int mineCount, IMinePlacer minePlacer)
        {
            Width = width;
            Height = height;
            this.mineCount = mineCount;
            this.minePlacer = minePlacer;

            cells = new Cell[width, height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                cells[x, y] = new Cell();
        }

        public Cell CellAt(Coordinate c) => cells[c.X, c.Y];

        public bool InBounds(Coordinate c) => c.X >= 0 && c.X < Width && c.Y >= 0 && c.Y < Height;

        /// <summary>Reveals a cell (placing mines on the first reveal) and returns every cell newly opened.</summary>
        public IReadOnlyList<Coordinate> Reveal(Coordinate origin)
        {
            var revealed = new List<Coordinate>();
            if (Status != GameStatus.InProgress || !InBounds(origin))
                return revealed;
            if (CellAt(origin).State != CellState.Hidden)
                return revealed;

            if (!minesPlaced)
                PlaceMines(origin);

            if (CellAt(origin).IsMine)
            {
                CellAt(origin).State = CellState.Revealed;
                revealed.Add(origin);
                Status = GameStatus.Lost;
                return revealed;
            }

            Flood(origin, revealed);

            if (safeRevealed == safeTotal)
                Status = GameStatus.Won;

            return revealed;
        }

        public void ToggleFlag(Coordinate c)
        {
            if (Status != GameStatus.InProgress || !InBounds(c))
                return;

            var cell = CellAt(c);
            if (cell.State == CellState.Hidden)
                cell.State = CellState.Flagged;
            else if (cell.State == CellState.Flagged)
                cell.State = CellState.Hidden;
        }

        private void PlaceMines(Coordinate safe)
        {
            int placed = 0;
            foreach (var mine in minePlacer.Place(Width, Height, mineCount, safe))
            {
                var cell = CellAt(mine);
                if (!cell.IsMine)
                {
                    cell.IsMine = true;
                    placed++;
                }
            }

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var c = new Coordinate(x, y);
                if (!CellAt(c).IsMine)
                    CellAt(c).AdjacentMines = CountAdjacentMines(c);
            }

            safeTotal = (Width * Height) - placed;
            minesPlaced = true;
        }

        // Iterative flood fill: opens the origin and cascades through 0-adjacent neighbours.
        private void Flood(Coordinate origin, List<Coordinate> revealed)
        {
            var stack = new Stack<Coordinate>();
            stack.Push(origin);
            while (stack.Count > 0)
            {
                var c = stack.Pop();
                var cell = CellAt(c);
                if (cell.State != CellState.Hidden || cell.IsMine)
                    continue;

                cell.State = CellState.Revealed;
                revealed.Add(c);
                safeRevealed++;

                if (cell.AdjacentMines == 0)
                    foreach (var n in Neighbors(c))
                        if (CellAt(n).State == CellState.Hidden)
                            stack.Push(n);
            }
        }

        private int CountAdjacentMines(Coordinate c)
        {
            int count = 0;
            foreach (var n in Neighbors(c))
                if (CellAt(n).IsMine)
                    count++;
            return count;
        }

        private IEnumerable<Coordinate> Neighbors(Coordinate c)
        {
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                var n = new Coordinate(c.X + dx, c.Y + dy);
                if (InBounds(n))
                    yield return n;
            }
        }
    }
}
