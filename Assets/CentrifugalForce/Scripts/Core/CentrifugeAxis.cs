using UnityEngine;

namespace CentrifugalForce
{
    /// <summary>
    /// Core component. Defines the rotation axis and angular velocity.
    /// Place on any GameObject — the axis will be this object's Transform.up.
    /// </summary>
    public class CentrifugeAxis : MonoBehaviour
    {
        [Header("Rotation")]
        [Tooltip("Angular velocity in RPM (revolutions per minute)")]
        [SerializeField] private float _rpm = 30f;

        [Tooltip("Ramp up time from 0 to target RPM (seconds)")]
        [SerializeField] private float _accelerationTime = 3f;

        [Header("Gravity Override")]
        [Tooltip("Disable Unity global gravity for all Rigidbodies in zone")]
        [SerializeField] private bool _overrideGlobalGravity = true;

        // Public read-only access
        public float AngularVelocityRad { get; private set; }
        public float CurrentRPM         { get; private set; }
        public Vector3 AxisDirection    => transform.up;
        public Vector3 AxisPosition     => transform.position;

        private float _targetOmega;
        private float _velocityRef;

        private void OnEnable()
        {
            if (_overrideGlobalGravity)
                Physics.gravity = Vector3.zero;
        }

        private void OnDisable()
        {
            if (_overrideGlobalGravity)
                Physics.gravity = new Vector3(0f, -9.81f, 0f);
        }

        private void Update()
        {
            _targetOmega = _rpm * Mathf.PI * 2f / 60f;

            AngularVelocityRad = Mathf.SmoothDamp(
                AngularVelocityRad,
                _targetOmega,
                ref _velocityRef,
                _accelerationTime
            );

            CurrentRPM = AngularVelocityRad * 60f / (Mathf.PI * 2f);
        }

        /// <summary>
        /// Change target RPM at runtime (e.g. from SpeedController UI)
        /// </summary>
        public void SetRPM(float rpm) => _rpm = Mathf.Max(0f, rpm);

        /// <summary>
        /// Calculates centrifugal acceleration at a given world position.
        /// Formula: a = ω² × r (where r is perpendicular distance from axis)
        /// </summary>
        public float GetAccelerationAt(Vector3 worldPosition)
        {
            Vector3 toPoint = worldPosition - AxisPosition;
            Vector3 radial = toPoint - Vector3.Dot(toPoint, AxisDirection) * AxisDirection;
            float r = radial.magnitude;
            return AngularVelocityRad * AngularVelocityRad * r;
        }

        /// <summary>
        /// Returns the outward direction from the axis at a given world position.
        /// </summary>
        public Vector3 GetRadialDirection(Vector3 worldPosition)
        {
            Vector3 toPoint = worldPosition - AxisPosition;
            Vector3 radial = toPoint - Vector3.Dot(toPoint, AxisDirection) * AxisDirection;
            return radial.magnitude > 0.001f ? radial.normalized : transform.right;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(
                transform.position - transform.up * 5f,
                transform.position + transform.up * 5f
            );
            Gizmos.DrawWireSphere(transform.position, 0.15f);
        }
#endif
    }
}