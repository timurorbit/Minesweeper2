using System.Collections.Generic;

namespace Minesweeper.Domain
{
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

        /// <summary>Returns the board to a fresh, unrevealed state (mines re-placed on the next reveal).</summary>
        public void Reset()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var cell = cells[x, y];
                cell.IsMine = false;
                cell.AdjacentMines = 0;
                cell.State = CellState.Hidden;
            }
            minesPlaced = false;
            safeRevealed = 0;
            safeTotal = 0;
        }

        /// <summary>Reveals a cell (placing mines on the first reveal) and reports what happened.</summary>
        public RevealResult Reveal(Coordinate origin)
        {
            var revealed = new List<Coordinate>();
            if (!InBounds(origin) || CellAt(origin).State != CellState.Hidden)
                return new RevealResult(revealed, hitMine: false, cleared: false);

            if (!minesPlaced)
                PlaceMines(origin);

            if (CellAt(origin).IsMine)
            {
                CellAt(origin).State = CellState.Revealed;
                revealed.Add(origin);
                return new RevealResult(revealed, hitMine: true, cleared: false);
            }

            Flood(origin, revealed);
            return new RevealResult(revealed, hitMine: false, cleared: safeRevealed == safeTotal);
        }

        public void ToggleFlag(Coordinate c)
        {
            if (!InBounds(c))
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
