using System;
using UnityEngine;
using PromVR.Utils;

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
            JsonStorage.SaveAsync(snapshot, snapshotFileName);
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

            TryLoadAsync();
        }

        private async Awaitable TryLoadAsync()
        {
            var snapshot = await JsonStorage.LoadAsync<DrawingBoardSnapshot>(snapshotFileName);

            if (snapshot?.Segments?.Length > 0)
            {
                drawingBoard.Clear();
                await drawingBoard.ApplySnapshotAsync(snapshot);
            }

            isLoading = false;
            DoneLoading?.Invoke();
        }
    }
}
