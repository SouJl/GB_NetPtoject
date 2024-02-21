using Configs;
using Enumerators;
using Photon.Pun;
using UI;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerMasterController : IPlayerController
    {
        private readonly PlayerConfig _data;
        private readonly PlayerView _view;
        private readonly PlayerViewUI _gameSceneUI;

        private bool _readyToJump;
        private bool _isGrounded;
        private bool _isResetJump;

        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        private float _resetJumpCount;

        private PlayerState _state;

        public string PlayerId { get; private set; }

        public PlayerState State => _state;

        private float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;

                    _view.PlayerUI.ChangeHealth(value);
                }
            }
        }

        private int _playerLevel;
        public int PlayerLevel
        {
            get => _playerLevel;
            set
            {
                _playerLevel = value;

                _view.PlayerUI.ChangeLevel(value);
            }
        }

        public float DamageDistance => _data.DamageDistance;

        public PlayerMasterController(
            string playerId,
            PlayerConfig data,
            PlayerView view,
            PlayerViewUI gameSceneUI,
            Camera camera)
        {
            PlayerId = playerId;

            _data = data;
            _view = view;

            _gameSceneUI = gameSceneUI;

            _currentHealth = data.MaxHealth;

            _view.Init(this, camera);

            var weaponData = data.WeaponData;
            _view.Weapon.Init(weaponData);
            _view.Weapon.OnAmmoChanged += WeponAmmoChanged;

            _gameSceneUI.InitUI(_view.photonView.Owner.NickName, data.MaxHealth, weaponData.MagSize);

            _readyToJump = true;
            _isResetJump = false;

            _state = PlayerState.Alive;
        }

        private void WeponAmmoChanged(int ammoValue)
        {
            _gameSceneUI.ChangeCurrentAmmo(ammoValue);
        }

        public void ChangeHealthValue(float value)
        {
            _gameSceneUI.ChangeHealth(value);

            CurrentHealth = value;

            if(CurrentHealth <= 0)
            {
                _state = PlayerState.Dead;

                Object.Instantiate(_data.PlayerOnDeathEffect, _view.SelfTransform.position, _view.SelfTransform.rotation);
                Object.Instantiate(_data.PlayerDeadPrefab, _view.SelfTransform.position, _view.SelfTransform.rotation);

                _view.Weapon.gameObject.SetActive(false);
            }
        }

        #region In Update Behaviour

        public void ExecuteUpdate(float deltaTime)
        {
            if (_view.photonView.IsMine == false)
                return;

            switch (_state)
            {
                case PlayerState.Alive:
                    {
                        UpdateWhenAlive(deltaTime);

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
            _isGrounded = Physics.Raycast(_view.SelfTransform.position, Vector3.down, _data.PlayerHeight * 0.5f + 0.3f, _data.WhatIsGround);

            MyInput();
            SpeedControl();

            if (_isGrounded)
            {
                _view.PlayerRb.drag = _data.GroundDrag;
            }
            else
            {
                _view.PlayerRb.drag = 0;
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
            _view.PlayerRb.velocity = new Vector3(_view.PlayerRb.velocity.x, 0f, _view.PlayerRb.velocity.z);

            _view.PlayerRb.AddForce(_view.SelfTransform.up * _data.JumpForce, ForceMode.Impulse);
        }

        private void SpeedControl()
        {
            var flatVel
                = new Vector3(_view.PlayerRb.velocity.x, 0f, _view.PlayerRb.velocity.z);

            if (flatVel.magnitude > _data.MoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _data.MoveSpeed;
                _view.PlayerRb.velocity = new Vector3(limitedVel.x, _view.PlayerRb.velocity.y, limitedVel.z);
            }
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }

        #endregion

        #region In FixedUpdate Behaviour

        public void ExecuteFixedUpdate(float fixedDeltaTime)
        {
            if (_view.photonView.IsMine == false)
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
                = _view.Orientation.forward * _verticalInput + _view.Orientation.right * _horizontalInput;

            if (_moveDirection != Vector3.zero)
                _view.PlayerObj.forward = Vector3.Slerp(_view.PlayerObj.forward, _moveDirection.normalized, Time.deltaTime * _data.RotationSpeed);


            if (_isGrounded)
            {
                _view.PlayerRb.AddForce(_moveDirection.normalized * _data.MoveSpeed * 10f, ForceMode.Force);
            }
            else if (!_isGrounded)
            {
                _view.PlayerRb.AddForce(_moveDirection.normalized * _data.MoveSpeed * 10f * _data.AirMultiplier, ForceMode.Force);
            }
        }

        #endregion

        #region IDisposable

        private bool _isDisposed = false;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            _view.Weapon.OnAmmoChanged += WeponAmmoChanged;

            Object.Destroy(_view.gameObject);
        }

        #endregion
    }
}
