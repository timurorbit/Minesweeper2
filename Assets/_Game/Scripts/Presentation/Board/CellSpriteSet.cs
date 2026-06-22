using Minesweeper.Domain;
using UnityEngine;

namespace Minesweeper.Presentation
{
    [CreateAssetMenu(fileName = "CellSpriteSet", menuName = "Minesweeper/Cell Sprite Set")]
    public sealed class CellSpriteSet : ScriptableObject
    {
        [SerializeField] private Sprite hidden;
        [SerializeField] private Sprite flagged;
        [SerializeField] private Sprite mine;
        [SerializeField] private Sprite[] numbers; // index = adjacent mine count (0..8)

        public Sprite SpriteFor(Cell cell)
        {
            if (cell.State == CellState.Hidden)
                return hidden;
            if (cell.State == CellState.Flagged)
                return flagged;
            return cell.IsMine ? mine : numbers[cell.AdjacentMines];
        }
    }
}
