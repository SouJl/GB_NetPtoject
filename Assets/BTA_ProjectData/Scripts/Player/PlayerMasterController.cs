using Abstraction;
using Configs;
using UI;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerMasterController : IPlayerController
    {
        private readonly PlayerConfig _data;
        private readonly PlayerView _view;
        private readonly GameSceneUI _gameSceneUI;


        private bool _readyToJump;
        private bool _isGrounded;
        private bool _isResetJump;

        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        private float _resetJumpCount;

        public string PlayerId { get; private set; }

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
            GameSceneUI gameSceneUI,
            Camera camera)
        {
            PlayerId = playerId;

            _data = data;
            _view = view;

            _gameSceneUI = gameSceneUI;

            _currentHealth = data.MaxHealth;

            _view.Init(this, camera);

            _gameSceneUI.InitUI(_view.photonView.Owner.NickName, data.MaxHealth);

            _readyToJump = true;
            _isResetJump = false;
        }

        public void ChangeHealthValue(float value)
        {
            _gameSceneUI.ChangeHealth(value);

            CurrentHealth = value;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if (_view.photonView.IsMine == false)
                return;

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

        public void ExecuteFixedUpdate(float fixedDeltaTime)
        {
            if (_view.photonView.IsMine == false)
                return;

            MovePlayer();
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

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            if (Physics.Raycast(_view.MainCamera.transform.position, _view.MainCamera.transform.forward, out var hit, _data.DamageDistance, _data.TargetsMask))
            {
                var target = hit.collider.gameObject.GetComponentInParent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(_data.DamageValue);
                }
            }
        }

        private void MovePlayer()
        {
            _moveDirection
                = _view.Orientation.forward * _verticalInput + _view.Orientation.right * _horizontalInput;

            if (_isGrounded)
            {
                _view.PlayerRb.AddForce(_moveDirection.normalized * _data.MoveSpeed * 10f, ForceMode.Force);
            }
            else if (!_isGrounded)
            {
                _view.PlayerRb.AddForce(_moveDirection.normalized * _data.MoveSpeed * 10f * _data.AirMultiplier, ForceMode.Force);
            }
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

        private void Jump()
        {
            _view.PlayerRb.velocity = new Vector3(_view.PlayerRb.velocity.x, 0f, _view.PlayerRb.velocity.z);

            _view.PlayerRb.AddForce(_view.SelfTransform.up * _data.JumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }

        #region IDisposable

        private bool _isDisposed = false;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            Object.Destroy(_view.gameObject);
        }

        #endregion
    }
}
