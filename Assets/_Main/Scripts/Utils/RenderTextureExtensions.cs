using UnityEngine;

namespace PromVR.Utils
{
    public static class RenderTextureExtensions
    {
        public static void FillWithWhite(this RenderTexture renderTexture)
        {
            Graphics.Blit(Texture2D.whiteTexture, renderTexture);
        }
    }
}