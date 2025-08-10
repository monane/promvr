using System.Collections.Generic;
using UnityEngine;
using PromVR.Utils;

namespace PromVR.Drawing
{
    public class RenderTextureDrawing : MonoBehaviour
    {
        [SerializeField] private Vector2Int drawableTextureResolution = new(1536, 1536);
        [SerializeField] private Material brushMaterial;
        [SerializeField] private Material brushBatchedMaterial;
        [SerializeField, Range(64, 256)] private int maxBatchSize = 64;

        public RenderTexture DrawableTexture { get; private set; }
        private RenderTexture drawableTextureBuffer;

        private BrushParams activeBrushParams;

        private void Awake()
        {
            InitDrawableTexture();
        }

        private void InitDrawableTexture()
        {
            DrawableTexture = new RenderTexture(
                drawableTextureResolution.x,
                drawableTextureResolution.y,
                0,
                RenderTextureFormat.ARGB32
            );

            DrawableTexture.Create();
            DrawableTexture.FillWithWhite();

            drawableTextureBuffer = new RenderTexture(DrawableTexture);
            drawableTextureBuffer.Create();

            brushMaterial.SetTexture("_MainTex", DrawableTexture);
            brushBatchedMaterial.SetTexture("_MainTex", DrawableTexture);
        }

        private void ApplyBrushParams(BrushParams brushParams)
        {
            var brushSizeParam = "_BrushSize";
            var brushColorParam = "_BrushColor";

            if (!activeBrushParams.Equals(brushParams))
            {
                brushMaterial.SetFloat(brushSizeParam, brushParams.Radius);
                brushMaterial.SetColor(brushColorParam, brushParams.Color);

                brushBatchedMaterial.SetFloat(brushSizeParam, brushParams.Radius);
                brushBatchedMaterial.SetColor(brushColorParam, brushParams.Color);

                activeBrushParams = brushParams;
            }
        }

        public void Draw(Vector2 uvPosition, BrushParams brushParams)
        {
            ApplyBrushParams(brushParams);

            brushMaterial.SetVector("_UVPosition", new Vector4(uvPosition.x, uvPosition.y));

            Graphics.Blit(DrawableTexture, drawableTextureBuffer, brushMaterial);
            Graphics.Blit(drawableTextureBuffer, DrawableTexture);
        }

        public async Awaitable Draw(List<Vector2> uvPositions, BrushParams brushParams)
        {
            ApplyBrushParams(brushParams);

            int totalPositions = uvPositions.Count;

            for (int start = 0; start < totalPositions; start += maxBatchSize)
            {
                int batchSize = Mathf.Min(maxBatchSize, totalPositions - start);

                var batchPositions = new Vector4[maxBatchSize];
                for (int i = 0; i < batchSize; i++)
                {
                    batchPositions[i] = uvPositions[start + i];
                }

                brushBatchedMaterial.SetInt("_PointCount", batchSize);
                brushBatchedMaterial.SetVectorArray("_UVPositions", batchPositions);

                Graphics.Blit(DrawableTexture, drawableTextureBuffer, brushBatchedMaterial);
                Graphics.Blit(drawableTextureBuffer, DrawableTexture);

                await Awaitable.NextFrameAsync();
            }
        }

        public void Clear()
        {
            DrawableTexture.FillWithWhite();
        }
    }
}
