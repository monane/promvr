using System;
using System.Collections.Generic;
using UnityEngine;
using PromVR.Utils;

namespace PromVR.Drawing
{
    public class DrawingBoard : MonoBehaviour
    {
        public event Action OnCleared;

        [SerializeField] private DrawingBoardControlPanel controlPanel;
        [SerializeField] private MeshRenderer boardMeshRenderer;

        [Header("Drawing Settings")]
        [SerializeField] private Vector2Int drawableTextureResolution = new(1536, 1536);
        [SerializeField] private Material brushMaterial;

        [Header("Internal Raycasting Settings")]
        [SerializeField] private LayerMask drawingBoardLayerMask;
        [SerializeField] private float pointerPositionOffset = -0.01f;
        [SerializeField] private float maxRayDistance = 0.75f;

        private readonly List<DrawingSegment> segments = new();

        private RenderTexture drawableTexture;
        private RenderTexture drawableTextureBuffer;

        private BrushParams? activeBrushParams;

        private void Awake()
        {
            InitDrawableTexture();
        }

        private void OnEnable()
        {
            controlPanel.ClearRequested += Clear;
        }

        private void OnDisable()
        {
            controlPanel.ClearRequested -= Clear;
        }

        private void InitDrawableTexture()
        {
            drawableTexture = new RenderTexture(
                drawableTextureResolution.x,
                drawableTextureResolution.y,
                0,
                RenderTextureFormat.ARGB32
            );

            drawableTexture.Create();
            drawableTexture.FillWithWhite();

            drawableTextureBuffer = new RenderTexture(drawableTexture);
            drawableTextureBuffer.Create();

            boardMeshRenderer.material.mainTexture = drawableTexture;

            brushMaterial.SetTexture("_MainTex", drawableTexture);
        }

        public void Clear()
        {
            segments.Clear();
            drawableTexture.FillWithWhite();
            OnCleared?.Invoke();
        }

        public void ApplyState(DrawingBoardSnapshot boardState)
        {
            foreach (var segment in boardState.Segments)
            {
                if (segment.Points.Count == 0)
                    continue;

                var index = InitNewSegment(segment.BrushParams);

                foreach (var point in segment.Points)
                {
                    DrawSegmentPoint(index, point);
                }

                segments.Add(segment);
            }
        }

        /// <returns>Index for created <c>DrawingSegment</c> item</returns>
        public int InitNewSegment(BrushParams brushParams)
        {
            segments.Add(new DrawingSegment
            {
                BrushParams = brushParams,
                Points = new List<Vector2>(capacity: 200)
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

        public void DrawSegmentPoint(int segmentIndex, Vector2 uvPosition)
        {
            var segment = segments[segmentIndex];

            ApplyBrushParams(segment.BrushParams);

            brushMaterial.SetVector(
                "_UVPosition",
                new Vector4(uvPosition.x, uvPosition.y, 0, 0)
            );

            Graphics.Blit(drawableTexture, drawableTextureBuffer, brushMaterial);
            Graphics.Blit(drawableTextureBuffer, drawableTexture);

            segment.Points.Add(uvPosition);
        }

        private void ApplyBrushParams(BrushParams brushParams)
        {
            if (!activeBrushParams.Equals(brushParams))
            {
                brushMaterial.SetFloat("_BrushSize", brushParams.Radius);
                brushMaterial.SetColor("_BrushColor", brushParams.Color);

                activeBrushParams = brushParams;
            }
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
