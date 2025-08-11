using System;
using System.Collections.Generic;
using UnityEngine;

namespace PromVR.Drawing.Segments
{
    [Serializable]
    public class DrawingSegment
    {
        public BrushParams BrushParams;
        public List<Vector2> Points;
    }
}
