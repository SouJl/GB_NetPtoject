using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(PlayerConfig), menuName = "B.T.A/" + nameof(PlayerConfig))]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Base Data")]
        [SerializeField]
        private float _maxHealth = 100f;

        [Space(10), Header("Movement")]
        [SerializeField]
        private float _moveSpeed = 7f;
        [SerializeField]
        private float _groundDrag = 5f;
        [SerializeField]
        private float _jumpForce = 12f;
        [SerializeField]
        private float _jumpCooldown = 0.25f;
        [SerializeField]
        private float _airMultiplier = 0.4f;
        [SerializeField]
        private float _rotationSpeed = 7f;

        [Space(10), Header("Attack")]
        [SerializeField]
        private float _damageValue = 10f;
        [SerializeField]
        private float _damageDistance = 100f;
        [SerializeField]
        private LayerMask _targetsMask;

        [Space(10), Header("Ground Check")]
        [SerializeField]
        private float _playerHeight = 1f;
        [SerializeField]
        private LayerMask _whatIsGround;

        [Space(10), Header("Keybinds")]
        [SerializeField]
        private KeyCode _jumpKey = KeyCode.Space;

        [SerializeField]
        private WeaponConfig _weaponData;

        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float GroundDrag => _groundDrag;
        public float JumpForce => _jumpForce;
        public float JumpCooldown => _jumpCooldown;
        public float AirMultiplier => _airMultiplier;
        public float RotationSpeed => _rotationSpeed;
        public float DamageValue => _damageValue;
        public float DamageDistance => _damageDistance;
        public LayerMask TargetsMask => _targetsMask;
        public float PlayerHeight => _playerHeight;
        public LayerMask WhatIsGround => _whatIsGround;
        public KeyCode JumpKey => _jumpKey;

        public WeaponConfig WeaponData => _weaponData;

    }

}
