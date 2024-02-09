using Photon.Pun;
using Tools;
using UI;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Base Info")]
        [SerializeField]
        private float _maxHealth = 100f;
        [SerializeField]
        private float _damageValue = 10f;
        [SerializeField]
        private float _damageDistance = 100f;
        [SerializeField]
        private Transform _firePoint;
        [SerializeField]
        private LayerMask _playermask;

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
        [SerializeField]
        private PlayerUI _playerUI;


        private GameSceneUI _gameSceneUI;

        private bool readyToJump;
        private bool _isGrounded;

        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        private Rigidbody _rb;
        private FirstPersonCamera _camera;

        public static GameObject LocalPlayerInstance;

        private Camera _mainCamera;


        private float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    _playerUI.ChangeHealth(value);
                    
                    if (_gameSceneUI)
                        _gameSceneUI.ChangeHealth(value);

                    Debug.Log($"Health : {value}");
                }
            }
        }

        private void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;

            _mainCamera = Camera.main;
        }

        private void Start()
        {
            readyToJump = true;

            var followedCamera = GetComponent<FirstPersonCamera>();

            if (followedCamera != null)
            {
                if (photonView.IsMine)
                {
                    followedCamera.Init(_mainCamera);
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            _currentHealth = _maxHealth;

            _playerUI.Init(_mainCamera, photonView.Owner.NickName, _maxHealth);

            if (photonView.IsMine)
            {
                _playerUI.Hide();
            }
        }

        private void Update()
        {
            if (photonView.IsMine == false)
                return;

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
            if (photonView.IsMine == false)
                return;

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

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out var hit, _damageDistance))
            {
                Debug.Log($"Hit Name: {hit.transform.name}");

                var target = hit.collider.gameObject.GetComponentInParent<PlayerController>();
                if (target != null)
                {
                    target.TakeDamage(_damageValue);
                }
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

        public void TakeDamage(float damageValue)
        {
            if (_currentHealth > 0)
            {
                CurrentHealth -= damageValue;
            }
        }

        public void SetGameUI(GameSceneUI gameSceneUI)
        {
            _gameSceneUI = gameSceneUI;
            _gameSceneUI.InitUI(photonView.Owner.NickName, _maxHealth);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CurrentHealth);
            }
            else
            {
                CurrentHealth = (float)stream.ReceiveNext();
            }
        }
    }
}
