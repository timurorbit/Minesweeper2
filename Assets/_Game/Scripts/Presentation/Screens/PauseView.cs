using System;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    public sealed class PauseView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        public event Action ResumeClicked;
        public event Action RestartClicked;
        public event Action MenuClicked;

        private void Awake()
        {
            resumeButton.onClick.AddListener(() => ResumeClicked?.Invoke());
            restartButton.onClick.AddListener(() => RestartClicked?.Invoke());
            menuButton.onClick.AddListener(() => MenuClicked?.Invoke());
        }

        public void SetVisible(bool visible) => panel.SetActive(visible);
    }
}
