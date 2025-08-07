using System.Collections.Generic;
using UnityEngine;

namespace PromVR.Drawing
{
    [System.Serializable]
    public struct DrawingSegment
    {
        public Color Color;
        public List<Vector2> Points;
    }
}
