using UnityEngine;

namespace CentrifugalForce
{
    [RequireComponent(typeof(Rigidbody))]
    public class CentrifugePlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 90f;

        [Header("Jump")]
        [SerializeField] private float _baseJumpForce = 5f;
        [SerializeField] private float _jumpDamping = 0.05f;

        [Header("Ground Check")]
        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask _groundLayer = ~0;

        [Header("References")]
        [SerializeField] private CentrifugeAxis _axis;

        private Rigidbody _rb;
        private bool _isGrounded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;                      
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            _isGrounded = CheckGrounded();
            HandleJump();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            float vertical = Input.GetAxis("Vertical");     // W/S
            float horizontal = Input.GetAxis("Horizontal"); // A/D

            if (Mathf.Abs(vertical) < 0.01f && Mathf.Abs(horizontal) < 0.01f) return;

            // Рух відносно камери — беремо forward і right з CinemachineCamera
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            // Прибираємо радіальну компоненту (рух тільки вздовж стінки)
            Vector3 radialDir = _axis.GetRadialDirection(transform.position);
            camForward -= Vector3.Dot(camForward, radialDir) * radialDir;
            camRight -= Vector3.Dot(camRight, radialDir) * radialDir;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * vertical + camRight * horizontal;
            
            if (moveDirection.sqrMagnitude < 0.01f) return;

            Vector3 targetVelocity = moveDirection.normalized * _moveSpeed;

            // Зберігаємо радіальну швидкість
            float radialSpeed = Vector3.Dot(_rb.linearVelocity, radialDir);
            _rb.linearVelocity = targetVelocity + radialDir * radialSpeed;
        }

        private void HandleRotation()
        {
            // Rotation тепер не потрібен — рух керується камерою
            // Можна залишити порожнім або видалити виклик з FixedUpdate
        }

        private void HandleJump()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            if (!_isGrounded) return;

            float rpm = _axis != null ? _axis.CurrentRPM : 0f;
            float jumpForce = rpm > 1f
                ? _baseJumpForce / (1f + rpm * _jumpDamping)
                : _baseJumpForce;

            Vector3 jumpDirection = -_axis.GetRadialDirection(transform.position);
            _rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
        }

        private bool CheckGrounded()
        {
            // Стріляємо від гравця до стінки = позитивний радіальний напрямок
            Vector3 toWall = _axis.GetRadialDirection(transform.position);
            bool grounded = Physics.Raycast(
                transform.position,
                toWall,
                _groundCheckDistance + 1f,
                _groundLayer
            );
            return grounded;
        }
    }
}