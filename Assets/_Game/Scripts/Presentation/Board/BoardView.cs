using System;
using Minesweeper.Domain;
using UnityEngine;

namespace Minesweeper.Presentation
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private CellSpriteSet sprites;
        [SerializeField] private float cellSize = 48f;

        private CellView[,] cells;
        private Board board;

        public event Action<Coordinate> CellLeftClicked;
        public event Action<Coordinate> CellRightClicked;

        public void Build(Board board)
        {
            this.board = board;
            CreateCells();
            RenderAll();
        }

        public void RenderAll()
        {
            for (int y = 0; y < board.Height; y++)
            for (int x = 0; x < board.Width; x++)
                RenderCell(new Coordinate(x, y));
        }

        public void RenderCell(Coordinate c) => cells[c.X, c.Y].Render(sprites.SpriteFor(board.CellAt(c)));

        private void CreateCells()
        {
            if (cells != null)
                return;

            cells = new CellView[board.Width, board.Height];
            for (int y = 0; y < board.Height; y++)
            for (int x = 0; x < board.Width; x++)
            {
                var coordinate = new Coordinate(x, y);
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.Initialize(coordinate);
                Place(cell, coordinate);
                cell.LeftClicked += RaiseLeft;
                cell.RightClicked += RaiseRight;
                cells[x, y] = cell;
            }
        }

        private void Place(CellView cell, Coordinate c)
        {
            var rect = (RectTransform)cell.transform;
            rect.sizeDelta = new Vector2(cellSize, cellSize);
            rect.anchoredPosition = new Vector2(c.X * cellSize, -c.Y * cellSize);
        }

        private void RaiseLeft(Coordinate c) => CellLeftClicked?.Invoke(c);
        private void RaiseRight(Coordinate c) => CellRightClicked?.Invoke(c);
    }
}
