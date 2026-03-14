using UnityEngine;

namespace VertigoSpin.Project.Scripts.Utils
{
    public static class VectorExtensions
    {
        public static Vector2 QuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float t)
        {
            float inv = 1f - t;
            return inv * inv * start
                   + 2f * inv * t * control
                   + t * t * end;
        }
    }
}
