using System;
using UnityEngine;
using Newtonsoft.Json;
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

        private JsonSerializerSettings jsonSerializerSettings;

        private bool isLoading;

        private void Awake()
        {
            InitJsonSerializerSettings();
        }

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

        private void InitJsonSerializerSettings()
        {
            jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new DrawingSegmentJsonCreationConverter());
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
            var snapshot = await JsonStorage.LoadAsync<DrawingBoardSnapshot>(
                snapshotFileName,
                jsonSerializerSettings
            );

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
