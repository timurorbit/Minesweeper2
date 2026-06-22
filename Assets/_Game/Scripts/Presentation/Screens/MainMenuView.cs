using System;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button startButton;

        public event Action StartClicked;

        private void Awake() => startButton.onClick.AddListener(() => StartClicked?.Invoke());

        public void SetVisible(bool visible) => panel.SetActive(visible);
    }
}
