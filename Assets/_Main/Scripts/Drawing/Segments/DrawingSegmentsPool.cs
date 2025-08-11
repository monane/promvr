using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PromVR.Drawing.Segments
{
    public static class DrawingSegmentsPool
    {
        private const int InitPointsListCapacity = 200;

        private static readonly ObjectPool<DrawingSegment> pool = new(
            createFunc: () => new DrawingSegment
            {
                Points = new List<Vector2>(InitPointsListCapacity)
            },
            actionOnRelease: segment => segment.Points.Clear()
        );

        public static DrawingSegment Get() => pool.Get();

        public static void Release(DrawingSegment segment) => pool.Release(segment);
    }
}
