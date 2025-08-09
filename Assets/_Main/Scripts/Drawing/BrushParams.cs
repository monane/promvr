using UnityEngine;

namespace PromVR.Drawing
{
    [System.Serializable]
    public struct BrushParams
    {
        public float Radius;
        public Color Color;

        public readonly bool Equals(BrushParams other)
        {
            return Radius == other.Radius && Color == other.Color;
        }
    }
}
