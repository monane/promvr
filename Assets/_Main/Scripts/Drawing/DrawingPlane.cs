using System.Collections.Generic;
using UnityEngine;

namespace PromVR.Drawing
{
    public class DrawingPlane : MonoBehaviour
    {
        [SerializeField] private DrawingPlaneControlPanel controlPanel;

        private readonly List<DrawingSegment> segments = new();

        private void OnEnable()
        {
            controlPanel.ClearRequested += Clear;
        }

        private void OnDisable()
        {
            controlPanel.ClearRequested -= Clear;
        }

        public void Clear()
        {
            segments.Clear();
            // also add texture clearing logic
        }

        public DrawingPlaneState CaptureState()
        {
            return new()
            {
                Segments = segments.ToArray()
            };
        }

        public void ApplyState(DrawingPlaneState planeState)
        {
            foreach (var segment in planeState.Segments)
            {
                segments.Add(segment);
                // also add texture rendering logic
            }
        }
    }
}
