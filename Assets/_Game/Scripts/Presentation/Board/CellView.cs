using System;
using Minesweeper.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Minesweeper.Presentation
{
    [RequireComponent(typeof(Image))]
    public sealed class CellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;

        public Coordinate Coordinate { get; private set; }

        public event Action<Coordinate> LeftClicked;
        public event Action<Coordinate> RightClicked;

        public void Initialize(Coordinate coordinate) => Coordinate = coordinate;

        public void Render(Sprite sprite) => image.sprite = sprite;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                LeftClicked?.Invoke(Coordinate);
            else if (eventData.button == PointerEventData.InputButton.Right)
                RightClicked?.Invoke(Coordinate);
        }
    }
}
