using UnityEngine;
using PromVR.Utils;

namespace PromVR.Drawing
{
    public class DrawingBoardPersistence : MonoBehaviour
    {
        [SerializeField] private DrawingBoard drawingBoard;
        [SerializeField] private DrawingBoardControlPanel controlPanel;

        private void OnEnable()
        {
            controlPanel.SaveRequested += SaveState;
            controlPanel.LoadRequested += TryLoadState;
        }

        private void OnDisable()
        {
            controlPanel.SaveRequested -= SaveState;
            controlPanel.LoadRequested -= TryLoadState;
        }

        private void SaveState()
        {
            var snapshot = drawingBoard.CaptureSnapshot();
            JsonStorage.TrySave(snapshot, nameof(DrawingBoardSnapshot));
        }

        private void TryLoadState()
        {
            if (JsonStorage.TryLoad(nameof(DrawingBoardSnapshot), out DrawingBoardSnapshot snapshot))
            {
                drawingBoard.Clear();
                drawingBoard.ApplyState(snapshot);
            }
        }
    }
}
