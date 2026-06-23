using UnityEngine;

namespace Minesweeper.Presentation.Config
{
    /// <summary>
    /// Designer-editable board settings.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Minesweeper/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        private const int MaxSize = 100;

        [SerializeField, Min(2)] private int width = 9;
        [SerializeField, Min(2)] private int height = 9;
        [SerializeField, Min(1)] private int mineCount = 10;

        public int Width => width;
        public int Height => height;
        public int MineCount => mineCount;

        /// <summary>At least the first-clicked cell must stay mine-free, so cap below cell count.</summary>
        public int MaxMines => (width * height) - 1;

        private void OnValidate()
        {
            width = Mathf.Clamp(width, 2, MaxSize);
            height = Mathf.Clamp(height, 2, MaxSize);
            mineCount = Mathf.Clamp(mineCount, 1, MaxMines);
        }
    }
}
