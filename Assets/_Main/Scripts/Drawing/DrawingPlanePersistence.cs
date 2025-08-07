using UnityEngine;
using PromVR.Utils;

namespace PromVR.Drawing
{
    public class DrawingPlanePersistence : MonoBehaviour
    {
        [SerializeField] private DrawingPlane drawingPlane;
        [SerializeField] private DrawingPlaneControlPanel controlPanel;

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
            var currentState = drawingPlane.CaptureState();
            JsonStorage.TrySave(currentState, nameof(DrawingPlaneState));
        }

        private void TryLoadState()
        {
            if (JsonStorage.TryLoad(nameof(DrawingPlaneState), out DrawingPlaneState loadedState))
            {
                drawingPlane.Clear();
                drawingPlane.ApplyState(loadedState);
            }
        }
    }
}
