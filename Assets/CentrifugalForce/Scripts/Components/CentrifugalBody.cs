using UnityEngine;

namespace CentrifugalForce
{
    /// <summary>
    /// Attach to any Rigidbody that should be affected by centrifugal force.
    /// Automatically registered via CentrifugeZone (trigger-based).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CentrifugalBody : MonoBehaviour
    {
        [Header("Force Settings")]
        [Tooltip("Use ω²r from axis (realistic) or fixed gravity value")]
        [SerializeField] private ForceMode _forceMode = ForceMode.Acceleration;

        [Tooltip("Multiplier on top of calculated centrifugal acceleration")]
        [SerializeField] private float _gravityMultiplier = 1f;

        [Tooltip("Optional fixed gravity (used when axis has RPM = 0 or for manual override)")]
        [SerializeField] private float _fallbackGravity = 9.81f;

        private Rigidbody _rb;
        private CentrifugeAxis _axis;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
        }

        /// <summary>Called by CentrifugeZone when body enters the field</summary>
        public void SetAxis(CentrifugeAxis axis) => _axis = axis;

        /// <summary>Called by CentrifugeZone when body exits</summary>
        public void ClearAxis() => _axis = null;

        private void FixedUpdate()
        {
            if (_axis == null) return;

            float acceleration = _axis.GetAccelerationAt(transform.position);
            acceleration *= _gravityMultiplier;

            if (acceleration > 0.01f)
            {
                Vector3 direction = _axis.GetRadialDirection(transform.position);
                _rb.AddForce(direction * acceleration, _forceMode);

                float radialVelocity = Vector3.Dot(_rb.linearVelocity, direction);
                if (radialVelocity < 0)
                {
                    _rb.AddForce(-direction * radialVelocity * 3f, ForceMode.Acceleration);
                }
            }
        }
    }
}