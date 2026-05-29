using UnityEngine;

namespace CentrifugalForce
{
    /// <summary>
    /// Controls the RPM of the centrifuge via Q/E keys.
    /// Attach to any GameObject in the scene.
    /// </summary>
    public class SpeedController : MonoBehaviour
    {
        [SerializeField] private CentrifugeAxis _axis;

        [Header("Speed Settings")]
        [Tooltip("How fast RPM changes per second")]
        [SerializeField] private float _rpmChangeRate = 5f;

        [Tooltip("Minimum allowed RPM")]
        [SerializeField] private float _minRPM = 0f;

        [Tooltip("Maximum allowed RPM")]
        [SerializeField] private float _maxRPM = 120f;

        private void Update()
        {
            if (_axis == null) return;

            float currentRPM = _axis.CurrentRPM;

            if (Input.GetKey(KeyCode.E))
                currentRPM += _rpmChangeRate * Time.deltaTime;

            if (Input.GetKey(KeyCode.Q))
                currentRPM -= _rpmChangeRate * Time.deltaTime;

            currentRPM = Mathf.Clamp(currentRPM, _minRPM, _maxRPM);
            _axis.SetRPM(currentRPM);
        }
    }
}