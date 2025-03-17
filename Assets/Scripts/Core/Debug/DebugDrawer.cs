using UnityEngine;
using Unity.Mathematics;

namespace Core.Debug
{
    public static class DebugDrawer
    {
        /// <summary>
        /// Draws a wireframe sphere using Debug.DrawLine
        /// </summary>
        /// <param name="center">Center position of the sphere</param>
        /// <param name="radius">Radius of the sphere</param>
        /// <param name="color">Color of the wireframe</param>
        /// <param name="duration">How long the sphere should be visible for (0 = one frame)</param>
        /// <param name="segments">Number of segments to use for each circle (higher = smoother)</param>
        public static void DrawWireSphere(float3 center, float radius, Color color, float duration = 0, int segments = 16)
        {
            for (int i = 0; i < segments; i++)
            {
                float angle1 = (float)i / segments * 2 * Mathf.PI;
                float angle2 = (float)(i + 1) / segments * 2 * Mathf.PI;

                // XY plane (vertical)
                var p1 = new Vector3(
                    center.x + radius * Mathf.Cos(angle1),
                    center.y + radius * Mathf.Sin(angle1),
                    center.z);
                var p2 = new Vector3(
                    center.x + radius * Mathf.Cos(angle2),
                    center.y + radius * Mathf.Sin(angle2),
                    center.z);
                UnityEngine.Debug.DrawLine(p1, p2, color, duration);

                // XZ plane (horizontal)
                p1 = new Vector3(
                    center.x + radius * Mathf.Cos(angle1),
                    center.y,
                    center.z + radius * Mathf.Sin(angle1));
                p2 = new Vector3(
                    center.x + radius * Mathf.Cos(angle2),
                    center.y,
                    center.z + radius * Mathf.Sin(angle2));
                UnityEngine.Debug.DrawLine(p1, p2, color, duration);

                // YZ plane
                p1 = new Vector3(
                    center.x,
                    center.y + radius * Mathf.Cos(angle1),
                    center.z + radius * Mathf.Sin(angle1));
                p2 = new Vector3(
                    center.x,
                    center.y + radius * Mathf.Cos(angle2),
                    center.z + radius * Mathf.Sin(angle2));
                UnityEngine.Debug.DrawLine(p1, p2, color, duration);
            }
        }
    }
}
