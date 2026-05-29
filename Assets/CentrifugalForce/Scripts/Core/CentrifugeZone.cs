using UnityEngine;
using System.Collections.Generic;

namespace CentrifugalForce
{
    /// <summary>
    /// Trigger collider that auto-registers/unregisters CentrifugalBody components.
    /// Attach alongside a Collider set to isTrigger = true.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CentrifugeZone : MonoBehaviour
    {
        [SerializeField] private CentrifugeAxis _axis;

        private readonly List<CentrifugalBody> _bodies = new();

        private void Reset()
        {
            _axis = GetComponentInParent<CentrifugeAxis>();
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<CentrifugalBody>(out var body))
            {
                body.SetAxis(_axis);
                _bodies.Add(body);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<CentrifugalBody>(out var body))
            {
                body.ClearAxis();
                _bodies.Remove(body);
            }
        }

        private void OnDisable()
        {
            foreach (var body in _bodies)
                body.ClearAxis();
            _bodies.Clear();
        }
    }
}