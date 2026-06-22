using System;
using Minesweeper.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    public sealed class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Image resultImage;
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite loseSprite;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        public event Action RestartClicked;
        public event Action MenuClicked;

        private void Awake()
        {
            restartButton.onClick.AddListener(() => RestartClicked?.Invoke());
            menuButton.onClick.AddListener(() => MenuClicked?.Invoke());
        }

        public void SetResult(GameStatus status)
        {
            bool won = status == GameStatus.Won;
            resultText.text = won ? "You win" : "You lose";
            if (resultImage != null)
                resultImage.sprite = won ? winSprite : loseSprite;
        }

        public void SetVisible(bool visible) => panel.SetActive(visible);
    }
}
