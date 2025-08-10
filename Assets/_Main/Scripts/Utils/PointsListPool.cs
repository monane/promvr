using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PromVR.Utils
{
    public static class PointsListPool
    {
        private static readonly int initListCapacity = 200;

        private static readonly ObjectPool<List<Vector2>> Pool = new(
            createFunc: () => new List<Vector2>(initListCapacity),
            actionOnRelease: list => list.Clear()
        );

        public static List<Vector2> Get() => Pool.Get();
        public static void Release(List<Vector2> item) => Pool.Release(item);
    }
}
