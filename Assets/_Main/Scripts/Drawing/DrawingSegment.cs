using System.Collections.Generic;
using UnityEngine;

namespace PromVR.Drawing
{
    [System.Serializable]
    public class DrawingSegment
    {
        public BrushParams BrushParams;
        public List<Vector2> Points;
    }
}
