using UnityEngine;

namespace BTAPlayer
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private float _groundDrag;
        [SerializeField]
        private float _jumpForce;
        [SerializeField]
        private float _jumpCooldown;
        [SerializeField]
        private float _airMultiplier;
 
        [Header("Keybinds")]
        [SerializeField]
        private KeyCode _jumpKey = KeyCode.Space;

        [Header("Ground Check")]
        [SerializeField]
        private float _playerHeight;
        [SerializeField]
        private LayerMask _whatIsGround;
        
        [Space(10), SerializeField]
        private Transform _orientation;

        private bool readyToJump;
        private bool _isGrounded;

        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;

            readyToJump = true;
        }

        private void Update()
        {
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.3f, _whatIsGround);

            MyInput();
            SpeedControl();
            if (_isGrounded)
                _rb.drag = _groundDrag;
            else
                _rb.drag = 0;
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(_jumpKey) && readyToJump && _isGrounded)
            {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }

        private void MovePlayer()
        {
            _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

            if (_isGrounded)
                _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);

            else if (!_isGrounded)
                _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            readyToJump = true;
        }
    }
}
