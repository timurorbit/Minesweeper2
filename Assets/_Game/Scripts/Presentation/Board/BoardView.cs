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
        [SerializeField, Range(0.5f, 1f)] private float fitPadding = 0.95f;

        private CellView[,] cells;
        private Board board;

        public event Action<Coordinate> CellLeftClicked;
        public event Action<Coordinate> CellRightClicked;

        public void Build(Board board)
        {
            this.board = board;
            CreateCells();
            FitGrid();
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
            var center = new Vector2(0.5f, 0.5f);
            rect.anchorMin = center;
            rect.anchorMax = center;
            rect.pivot = center;
            rect.sizeDelta = new Vector2(cellSize, cellSize);

            float originX = -(board.Width - 1) * cellSize * 0.5f;
            float originY = (board.Height - 1) * cellSize * 0.5f;
            rect.anchoredPosition = new Vector2(originX + c.X * cellSize, originY - c.Y * cellSize);
        }

        private void FitGrid()
        {
            float gridWidth = board.Width * cellSize;
            float gridHeight = board.Height * cellSize;
            gridRoot.sizeDelta = new Vector2(gridWidth, gridHeight);

            Vector2 area = ((RectTransform)gridRoot.parent).rect.size;
            float scale = Mathf.Min(area.x / gridWidth, area.y / gridHeight) * fitPadding;
            gridRoot.localScale = new Vector3(scale, scale, 1f);
        }

        private void RaiseLeft(Coordinate c) => CellLeftClicked?.Invoke(c);
        private void RaiseRight(Coordinate c) => CellRightClicked?.Invoke(c);
    }
}
