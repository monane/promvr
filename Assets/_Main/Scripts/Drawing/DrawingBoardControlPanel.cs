using System;
using UnityEngine;
using UnityEngine.UI;

namespace PromVR.Drawing
{
    public class DrawingBoardControlPanel : MonoBehaviour
    {
        public event Action ColorChangeRequested;
        public event Action SaveRequested;
        public event Action LoadRequested;
        public event Action ClearRequested;

        [SerializeField] private Button colorButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Image colorPreview;

        private void Awake()
        {
            saveButton.onClick.AddListener(() => SaveRequested?.Invoke());
            loadButton.onClick.AddListener(() => LoadRequested?.Invoke());
            clearButton.onClick.AddListener(() => ClearRequested?.Invoke());
            colorButton.onClick.AddListener(() => ColorChangeRequested?.Invoke());
        }

        public void SetColorPreview(Color color)
        {
            colorPreview.color = color;
        }
    }
}
