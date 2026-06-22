using TMPro;
using UnityEngine;

namespace Minesweeper.Presentation
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text statusText;

        private int shownSeconds = -1;

        public void SetTime(float seconds)
        {
            int whole = (int)seconds;
            if (whole == shownSeconds)
                return;
            shownSeconds = whole;
            timerText.text = whole.ToString();
        }

        public void SetStatus(string message) => statusText.text = message;

        public void Clear()
        {
            shownSeconds = -1;
            timerText.text = "0";
            statusText.text = string.Empty;
        }
    }
}
