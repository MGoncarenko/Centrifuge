using UnityEngine;

namespace CentrifugalForce
{
    /// <summary>
    /// Transfers tangential velocity from rotating cylinder to bodies touching the wall.
    /// Attach to CylinderMesh alongside RotatingCylinder.
    /// </summary>
    public class CylinderFriction : MonoBehaviour
    {
        [SerializeField] private CentrifugeAxis _axis;

        [Tooltip("How strongly the cylinder drags bodies along with it")]
        [SerializeField] private float _frictionStrength = 10f;

        private void OnCollisionStay(Collision collision)
        {
            if (!collision.rigidbody) return;

            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 contactPoint = contact.point;

                Vector3 toContact = contactPoint - _axis.AxisPosition;
                Vector3 radial = toContact - Vector3.Dot(toContact, _axis.AxisDirection) 
                                 * _axis.AxisDirection;
                Vector3 cylinderSurfaceVelocity = Vector3.Cross(
                    _axis.AxisDirection * _axis.AngularVelocityRad, radial);

                Vector3 bodyVelocity = collision.rigidbody.linearVelocity;

                Vector3 velocityDiff = cylinderSurfaceVelocity - bodyVelocity;

                Vector3 radialDir = _axis.GetRadialDirection(contactPoint);
                velocityDiff -= Vector3.Dot(velocityDiff, radialDir) * radialDir;

                collision.rigidbody.AddForce(
                    velocityDiff * _frictionStrength,
                    ForceMode.Acceleration
                );
            }
        }
    }
}