using Abstraction;
using Photon.Pun;
using UnityEngine;
using Enumerators;
using System.Collections.Generic;
using Configs;
using Tools;
using Weapon;

namespace BTAPlayer
{
    public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IFindable
    {
        [Header("Base Sttings")]

        [SerializeField]
        private PlayerConfig _data;
        [SerializeField]
        private Transform _selfTransform;
        [SerializeField]
        private Transform _orientation;
        [SerializeField]
        private Transform _playerObj;
        [SerializeField]
        private PlayerUI _playerUI;
        [SerializeField]
        private PlayerBodyObjects _body;
        [SerializeField]
        private FirstPersonCamera _cameraManager;
        [SerializeField]
        private WeaponController _weapon;

        private Rigidbody _playerRb;
        private Camera _mainCamera;

        private PlayerState _state;
        private string _nickName;
        private float _currentHealth;
        private int _currentLevel;

        private bool _readyToJump;
        private bool _isGrounded;
        private bool _isResetJump;
        private Vector3 _moveDirection;
        private float _resetJumpCount;
        private float _horizontalInput;
        private float _verticalInput;

        public Transform SelfTransform => _selfTransform;

        public PlayerState State => _state;

        public string NickName
        {
            get => _nickName;

            private set
            {
                _nickName = value;

                if (photonView.IsMine)
                    PlayerInput.ChangeName(value);
            }
        }

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;

                _playerUI.ChangeHealth(value);

                if (photonView.IsMine)
                    PlayerInput.ChangeHealth(value);
            }
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;

                _playerUI.ChangeLevel(value);

                if (photonView.IsMine)
                    PlayerInput.ChangeLevel(value);
            }
        }

        private void Awake()
        {
            _playerRb = GetComponent<Rigidbody>();
            _playerRb.freezeRotation = true;

            _mainCamera = Camera.main;

            _readyToJump = true;
            _isResetJump = false;

            if (!photonView.IsMine)
            {
                _body.ShowBody();
            }
            else
            {
                _body.HideBody();
            }
        }

        private void Start()
        {
            CurrentHealth = _data.MaxHealth;

            var instData = photonView.InstantiationData;

            NickName = (string)instData[0];
            CurrentLevel = (int)instData[1];

            _playerUI.Init(_mainCamera, NickName, _data.MaxHealth);

            if (photonView.IsMine)
            {
                _weapon.OnAmmoChanged += WeponAmmoChanged;

                if (_cameraManager != null)
                {
                    _cameraManager.Init(_mainCamera);
                }

                _playerUI.Hide();
            }

            _weapon.Init(_data.WeaponData);


            if (!photonView.IsMine)
            {
                _weapon.gameObject.SetActive(false);
            }

            photonView.RPC(nameof(UpdateLevelOnClient), RpcTarget.Others, new object[] { photonView.ViewID, CurrentLevel });

            _state = PlayerState.Alive;
        }

        private void WeponAmmoChanged(int ammoValue)
        {
            PlayerInput.ChangeAmmo(ammoValue);
        }

        #region In Update

        private void Update()
        {
            if (!photonView.IsMine)
                return;

            switch (_state)
            {
                case PlayerState.Alive:
                    {
                        UpdateWhenAlive(Time.deltaTime);

                        break;
                    }
                case PlayerState.Dead:
                    {
                        break;
                    }
            }
        }

        private void UpdateWhenAlive(float deltaTime)
        {
            _isGrounded = Physics.Raycast(_selfTransform.position, Vector3.down, _data.PlayerHeight * 0.5f + 0.3f, _data.WhatIsGround);

            MyInput();
            SpeedControl();

            if (_isGrounded)
            {
                _playerRb.drag = _data.GroundDrag;
            }
            else
            {
                _playerRb.drag = 0;
            }

            if (_isResetJump)
            {
                _resetJumpCount += deltaTime;

                if (_resetJumpCount >= _data.JumpCooldown)
                {
                    ResetJump();
                    _resetJumpCount = 0;
                    _isResetJump = false;
                }
            }
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(_data.JumpKey) && _readyToJump && _isGrounded)
            {
                _readyToJump = false;
                _isResetJump = true;

                Jump();
            }
        }

        private void Jump()
        {
            _playerRb.velocity = new Vector3(_playerRb.velocity.x, 0f, _playerRb.velocity.z);
            _playerRb.AddForce(_selfTransform.up * _data.JumpForce, ForceMode.Impulse);
        }

        private void SpeedControl()
        {
            var flatVel = new Vector3(_playerRb.velocity.x, 0f, _playerRb.velocity.z);

            if (flatVel.magnitude > _data.MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _data.MoveSpeed;
                _playerRb.velocity = new Vector3(limitedVel.x, _playerRb.velocity.y, limitedVel.z);
            }
        }
        private void ResetJump()
        {
            _readyToJump = true;
        }

        #endregion

        #region In FixedUpdate

        private void FixedUpdate()
        {
            if (!photonView.IsMine)
                return;

            switch (_state)
            {
                case PlayerState.Alive:
                    {
                        MovePlayer();
                        break;
                    }
                case PlayerState.Dead:
                    {
                        break;
                    }
            }
        }

        private void MovePlayer()
        {
            _moveDirection
                = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

            if (_moveDirection != Vector3.zero)
                _playerObj.forward = Vector3.Slerp(_playerObj.forward, _moveDirection.normalized, Time.deltaTime * _data.RotationSpeed);


            if (_isGrounded)
            {
                _playerRb.AddForce(_moveDirection.normalized * _data.MoveSpeed * 10f, ForceMode.Force);
            }
            else if (!_isGrounded)
            {
                _playerRb.AddForce(_moveDirection.normalized * _data.MoveSpeed * 10f * _data.AirMultiplier, ForceMode.Force);
            }
        }

        #endregion

        #region PUNRPC_METHODS

        [PunRPC]
        private void UpdateHealthOnClient(int id, float value)
        {
            if (photonView.ViewID != id)
                return;

            CurrentHealth = value;
        }

        [PunRPC]
        private void UpdateLevelOnClient(int id, int value)
        {
            if (photonView.ViewID != id)
                return;

            CurrentLevel = value;
        }

        #endregion

        #region IDamageable

        public void TakeDamage(DamageData damage)
        {
            if (_state == PlayerState.Dead)
                return;

            CurrentHealth -= damage.Value;

            photonView.RPC(nameof(UpdateHealthOnClient), RpcTarget.Others, new object[] { photonView.ViewID, CurrentHealth });
        }

        #endregion

        #region IFindable

        [Header("IFindable Settings")]
        [SerializeField]
        private List<Transform> _visiblePoints;

        Transform IFindable.Transform => transform;

        GameObject IFindable.GameObject => gameObject;

        List<Transform> IFindable.VisiblePoints => _visiblePoints;

        #endregion
    }
}
