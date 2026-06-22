using System;
using Minesweeper.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform gridRoot;     // cells parent + ScrollRect content (the background)
        [SerializeField] private RectTransform scrollArea;   // visible window; clamped to maxViewportSize
        [SerializeField] private CellView cellPrefab;
        [SerializeField] private CellSpriteSet sprites;
        [SerializeField] private float cellSize = 20f;
        [SerializeField] private float maxViewportSize = 400f;

        private CellView[,] cells;
        private Board board;

        public event Action<Coordinate> CellLeftClicked;
        public event Action<Coordinate> CellRightClicked;

        public void Build(Board board)
        {
            this.board = board;
            CreateCells();
            LayoutBoard();
            RenderAll();
        }

        public void RenderAll()
        {
            for (int y = 0; y < board.Height; y++)
            for (int x = 0; x < board.Width; x++)
                RenderCell(new Coordinate(x, y));
        }

        public void RenderCell(Coordinate c) => cells[c.X, c.Y].Render(sprites.SpriteFor(board.CellAt(c)));

        public void RenderExploded(Coordinate c) => cells[c.X, c.Y].Render(sprites.Exploded);

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
            var topLeft = new Vector2(0f, 1f);
            rect.anchorMin = topLeft;
            rect.anchorMax = topLeft;
            rect.pivot = topLeft;
            rect.sizeDelta = new Vector2(cellSize, cellSize);
            rect.anchoredPosition = new Vector2(c.X * cellSize, -c.Y * cellSize);
        }

        private void LayoutBoard()
        {
            float gridWidth = board.Width * cellSize;
            float gridHeight = board.Height * cellSize;
            gridRoot.sizeDelta = new Vector2(gridWidth, gridHeight);

            if (scrollArea == null)
                return;

            scrollArea.sizeDelta = new Vector2(
                Mathf.Min(gridWidth, maxViewportSize),
                Mathf.Min(gridHeight, maxViewportSize));

            if (scrollArea.TryGetComponent(out ScrollRect scroll))
            {
                scroll.horizontal = gridWidth > maxViewportSize;
                scroll.vertical = gridHeight > maxViewportSize;
            }
        }

        private void RaiseLeft(Coordinate c) => CellLeftClicked?.Invoke(c);
        private void RaiseRight(Coordinate c) => CellRightClicked?.Invoke(c);
    }
}
