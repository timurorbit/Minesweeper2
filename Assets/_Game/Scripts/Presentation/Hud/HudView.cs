using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private Button pauseButton;

        public event Action PauseClicked;

        private int shownSeconds = -1;

        private void Awake() => pauseButton.onClick.AddListener(() => PauseClicked?.Invoke());

        public void SetTime(float seconds)
        {
            int whole = (int)seconds;
            if (whole == shownSeconds)
                return;
            shownSeconds = whole;
            timerText.text = whole.ToString();
        }

        public void Clear()
        {
            shownSeconds = -1;
            timerText.text = "0";
        }
    }
}
