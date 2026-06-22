using System.Collections.Generic;

namespace Minesweeper.Domain
{
    public sealed class Board
    {
        private readonly Cell[,] cells;
        private readonly int mineCount;
        private readonly IMinePlacer minePlacer;
        private bool minesPlaced;

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
        }

        /// <summary>Reveals a cell (placing mines on the first reveal); a mine hit reveals every mine.</summary>
        public RevealResult Reveal(Coordinate origin)
        {
            var revealed = new List<Coordinate>();
            if (!InBounds(origin) || CellAt(origin).State != CellState.Hidden)
                return new RevealResult(revealed, hitMine: false);

            if (!minesPlaced)
                PlaceMines(origin);

            if (CellAt(origin).IsMine)
            {
                RevealAllMines(revealed);
                return new RevealResult(revealed, hitMine: true);
            }

            Flood(origin, revealed);
            return new RevealResult(revealed, hitMine: false);
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

        /// <summary>True once mines are placed and every safe cell is revealed (only mines remain unopened).</summary>
        public bool IsSolved()
        {
            if (!minesPlaced)
                return false;

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var cell = cells[x, y];
                if (!cell.IsMine && cell.State != CellState.Revealed)
                    return false;
            }
            return true;
        }

        private void PlaceMines(Coordinate safe)
        {
            foreach (var mine in minePlacer.Place(Width, Height, mineCount, safe))
                CellAt(mine).IsMine = true;

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var c = new Coordinate(x, y);
                if (!CellAt(c).IsMine)
                    CellAt(c).AdjacentMines = CountAdjacentMines(c);
            }

            minesPlaced = true;
        }

        private void RevealAllMines(List<Coordinate> revealed)
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var c = new Coordinate(x, y);
                var cell = CellAt(c);
                if (cell.IsMine && cell.State == CellState.Hidden)
                {
                    cell.State = CellState.Revealed;
                    revealed.Add(c);
                }
            }
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
