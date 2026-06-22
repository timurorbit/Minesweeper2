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
        [SerializeField] private Sprite exploded; // the mine the player clicked
        [SerializeField] private Sprite[] numbers; // index = adjacent mine count (0..8)

        public Sprite Exploded => exploded;

        public Sprite SpriteFor(Cell cell)
        {
            if (cell.State == CellState.Hidden)
                return hidden;
            if (cell.State == CellState.Flagged)
                return flagged;
            if (cell.IsMine)
                return mine;
            return numbers[cell.AdjacentMines];
        }
    }
}
