using System;
using Newtonsoft.Json.Converters;

namespace PromVR.Drawing.Segments
{
    public class DrawingSegmentJsonCreationConverter : CustomCreationConverter<DrawingSegment>
    {
        public override DrawingSegment Create(Type objectType)
        {
            return DrawingSegmentsPool.Get();
        }
    }
}
