using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Minesweeper.Presentation
{
    public sealed class GameInput : MonoBehaviour
    {
        [SerializeField] private Key restartKey = Key.R;

        public event Action RestartRequested;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[restartKey].wasPressedThisFrame)
                RestartRequested?.Invoke();
        }
    }
}
