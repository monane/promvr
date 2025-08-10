using System;
using System.Collections.Generic;
using UnityEngine;
using PromVR.Utils;

namespace PromVR.Drawing
{
    public class DrawingBoard : MonoBehaviour
    {
        public event Action Cleared;

        [SerializeField] private DrawingBoardControlPanel controlPanel;
        [SerializeField] private MeshRenderer boardMeshRenderer;
        [SerializeField] private RenderTextureDrawing renderTextureDrawing;

        [Header("Internal Raycasting Settings")]
        [SerializeField] private LayerMask drawingBoardLayerMask;
        [SerializeField] private float pointerPositionOffset = -0.01f;
        [SerializeField] private float maxRayDistance = 0.75f;

        private readonly List<DrawingSegment> segments = new();

        private void OnEnable()
        {
            controlPanel.ClearRequested += Clear;
        }

        private void OnDisable()
        {
            controlPanel.ClearRequested -= Clear;
        }

        private void Start()
        {
            boardMeshRenderer.material.mainTexture = renderTextureDrawing.DrawableTexture;
        }

        public void Clear()
        {
            foreach (var segment in segments)
            {
                PointsListPool.Release(segment.Points);
            }

            segments.Clear();
            renderTextureDrawing.Clear();
            Cleared?.Invoke();
        }

        public async Awaitable ApplySnapshotAsync(DrawingBoardSnapshot snapshot)
        {
            foreach (var segment in snapshot.Segments)
            {
                if (segment.Points.Count == 0)
                    continue;

                await renderTextureDrawing.Draw(segment.Points, segment.BrushParams);

                segments.Add(segment);
            }
        }

        /// <returns>Index for created <c>DrawingSegment</c> item</returns>
        public int InitNewSegment(BrushParams brushParams)
        {
            segments.Add(new DrawingSegment
            {
                BrushParams = brushParams,
                Points = PointsListPool.Get()
            });

            return segments.Count - 1;
        }

        public void DrawSegmentPoint(int segmentIndex, Pose pointerPose)
        {
            if (TryGetBoardUVPositionByPointerPose(pointerPose, out var uvPosition))
            {
                DrawSegmentPoint(segmentIndex, uvPosition);
            }
        }

        private bool TryGetBoardUVPositionByPointerPose(Pose pointerPose, out Vector2 uvPosition)
        {
            // For some reason, pointerPose.forward points in the opposite direction.
            var pointerDirection = -pointerPose.forward;
            var pointerPosition = pointerPose.position + pointerDirection * pointerPositionOffset;

            var ray = new Ray(pointerPosition, pointerDirection);

            if (Physics.Raycast(ray, out var rayHit, maxRayDistance, drawingBoardLayerMask))
            {
                uvPosition = rayHit.textureCoord;
                return true;
            }

            uvPosition = default;
            return false;
        }

        private void DrawSegmentPoint(int segmentIndex, Vector2 uvPosition)
        {
            var segment = segments[segmentIndex];

            renderTextureDrawing.Draw(uvPosition, segment.BrushParams);

            segment.Points.Add(uvPosition);
        }

        public DrawingBoardSnapshot CaptureSnapshot()
        {
            return new()
            {
                Segments = segments.ToArray()
            };
        }
    }
}
