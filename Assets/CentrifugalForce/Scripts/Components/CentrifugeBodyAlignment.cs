using UnityEngine;

namespace CentrifugalForce
{
    [RequireComponent(typeof(Rigidbody))]
    public class CentrifugeBodyAlignment : MonoBehaviour
    {
        [SerializeField] private CentrifugeAxis _axis;

        [Tooltip("How fast the body rotates to align with the wall")]
        [SerializeField] private float _alignmentSpeed = 5f;

        [Tooltip("Max distance to cast ray toward wall")]
        [SerializeField] private float _rayDistance = 10f;

        private Rigidbody _rb;
        private Vector3 _currentUp;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _currentUp = transform.up;
        }

        private void FixedUpdate()
        {
            if (_axis == null) return;

            Vector3 up = -_axis.GetRadialDirection(transform.position);

            Vector3 forward = _axis.AxisDirection;

            if (Vector3.Cross(up, forward).sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(forward, up);

            _rb.MoveRotation(targetRotation);
        }

        private Vector3 GetWallNormal()
        {
            return _axis.GetRadialDirection(transform.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_axis == null) return;

            Vector3 toWall = _axis.GetRadialDirection(transform.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + toWall * _rayDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + _currentUp * 2f);
        }
#endif
    }
}