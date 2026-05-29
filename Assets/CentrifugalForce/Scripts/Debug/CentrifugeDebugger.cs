using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CentrifugalForce
{
    /// <summary>
    /// Runtime HUD + Editor Gizmos for the centrifuge system.
    /// Safe to strip from production builds.
    /// </summary>
    public class CentrifugeDebugger : MonoBehaviour
    {
        [SerializeField] private CentrifugeAxis _axis;
        [SerializeField] private bool _showHUD = true;

        private GUIStyle _style;

        private void OnGUI()
        {
            if (!_showHUD || _axis == null) return;

            _style ??= new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(10, 10, 10, 10)
            };

            string info =
                $"RPM: {_axis.CurrentRPM:F1}\n" +
                $"ω: {_axis.AngularVelocityRad:F2} rad/s\n" +
                $"Gravity override: {(Physics.gravity == Vector3.zero ? "ON" : "OFF")}";

            GUI.Box(new Rect(10, 10, 220, 80), info, _style);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_axis == null) return;

            // Draw radial force arrows in a ring around axis
            Handles.color = new Color(0f, 1f, 1f, 0.4f);
            int steps = 16;
            float radius = 3f;
            for (int i = 0; i < steps; i++)
            {
                float angle = i * 360f / steps * Mathf.Deg2Rad;
                Vector3 origin = _axis.AxisPosition + new Vector3(
                    Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                Vector3 dir = _axis.GetRadialDirection(origin);
                Handles.DrawLine(origin, origin + dir * 0.8f);
            }
        }
#endif
    }
}