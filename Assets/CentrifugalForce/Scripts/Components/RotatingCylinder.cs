using UnityEngine;

namespace CentrifugalForce
{
    /// <summary>
    /// Rotates the visual cylinder mesh around the axis.
    /// Reads ω from CentrifugeAxis so visuals match physics exactly.
    /// </summary>
    public class RotatingCylinder : MonoBehaviour
    {
        [SerializeField] private CentrifugeAxis _axis;

        [Tooltip("Rotate this Transform (the cylinder mesh)")]
        [SerializeField] private Transform _cylinderMesh;

        private void Reset()
        {
            _axis = GetComponentInParent<CentrifugeAxis>();
            _cylinderMesh = transform;
        }

        private void Update()
        {
            if (_axis == null || _cylinderMesh == null) return;

            float degreesThisFrame = _axis.AngularVelocityRad
                                     * Mathf.Rad2Deg
                                     * Time.deltaTime;

            _cylinderMesh.Rotate(_axis.AxisDirection, degreesThisFrame, Space.World);
        }
    }
}