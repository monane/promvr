using System.Collections.Generic;
using PromVR.Utils;
using UnityEngine;

namespace PromVR.Drawing
{
    public class DrawingBoard : MonoBehaviour
    {
        [SerializeField] private DrawingBoardControlPanel controlPanel;
        [SerializeField] private MeshRenderer boardMeshRenderer;

        [Header("Drawing settings")]
        [SerializeField] private Vector2Int drawableTextureResolution;
        [SerializeField] private Material brushMaterial;

        private readonly List<DrawingSegment> segments = new();

        private RenderTexture drawableTexture;
        private RenderTexture drawableTextureBuffer;

        private BrushParams? activeBrushParams;

        private void Awake()
        {
            InitDrawableTexture();
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
            drawableTexture.FillWithWhite();
        }

        public DrawingBoardSnapshot CaptureSnapshot()
        {
            return new()
            {
                Segments = segments.ToArray()
            };
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

        public void DrawSegmentPoint(int segmentIndex, Vector3 worldPosition)
        {
            Vector2 uvPosition = worldPosition;

            segments[segmentIndex].Points.Add(uvPosition);
        }

        public void DrawSegmentPoint(int segmentIndex, Vector2 uvPosition)
        {
            var segment = segments[segmentIndex];

            ApplyBrushParams(segment.BrushParams);

            segment.Points.Add(uvPosition);

            brushMaterial.SetVector(
                "_UVPosition",
                new Vector4(uvPosition.x, uvPosition.y, 0, 0)
            );

            Graphics.Blit(drawableTexture, drawableTextureBuffer, brushMaterial);
            Graphics.Blit(drawableTextureBuffer, drawableTexture);
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
    }
}
