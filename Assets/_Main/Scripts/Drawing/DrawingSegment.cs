using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Converters;

namespace PromVR.Drawing
{
    [Serializable]
    public class DrawingSegment
    {
        public BrushParams BrushParams;
        public List<Vector2> Points;
    }

    public class DrawingSegmentJsonCreationConverter : CustomCreationConverter<DrawingSegment>
    {
        public override DrawingSegment Create(Type objectType)
        {
            return DrawingSegmentsPool.Get();
        }
    }
}
