using System;
using UnityEngine;
using PromVR.Utils;
using Cysharp.Threading.Tasks;

namespace PromVR.Drawing
{
    public class DrawingBoardPersistence : MonoBehaviour
    {
        public event Action BeginLoading;
        public event Action DoneLoading;

        [SerializeField] private DrawingBoard drawingBoard;
        [SerializeField] private DrawingBoardControlPanel controlPanel;
        [SerializeField] private string snapshotFileName = "snapshot";

        private bool isLoading;

        private void OnEnable()
        {
            controlPanel.SaveRequested += SaveState;
            controlPanel.LoadRequested += OnLoadRequested;
        }

        private void OnDisable()
        {
            controlPanel.SaveRequested -= SaveState;
            controlPanel.LoadRequested -= OnLoadRequested;
        }

        private void SaveState()
        {
            var snapshot = drawingBoard.CaptureSnapshot();
            JsonStorage.SaveAsync(snapshot, snapshotFileName).Forget();
        }

        private void OnLoadRequested()
        {
            if (isLoading)
            {
                Debug.LogError("Already loading DrawingBoardSnapshot!", this);
                return;
            }

            isLoading = true;
            BeginLoading?.Invoke();

            JsonStorage.LoadAsync<DrawingBoardSnapshot>(snapshotFileName).ContinueWith(snapshot =>
            {
                if (snapshot?.Segments?.Length > 0)
                {
                    drawingBoard.Clear();
                    drawingBoard.ApplySnapshot(snapshot);
                }

                isLoading = false;
                DoneLoading?.Invoke();
            });
        }
    }
}
