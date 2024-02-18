using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(EnemyConfig), menuName = "B.T.A/" + nameof(EnemyConfig))]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField]
        private float _maxHealth = 100f;
        [SerializeField]
        private float _moveSpeed = 5f;
        [SerializeField]
        private float _deadDelay = 1f;

        [Space(10), Header("Attack Data")]
        [SerializeField]
        private float _damageValue = 20f;
        [SerializeField]
        private float _damageDistance = 5f;
        [SerializeField]
        private LayerMask _attackTargetMask;

        [Space(10), Header("Find Target Data")]
        [SerializeField]
        private float _searchAngle = 90f;
        [SerializeField]
        private float _searchDistance = 10f;
        [SerializeField]
        private LayerMask _searchMask;

        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float DeadDelay  => _deadDelay;
        public float DamageValue => _damageValue;
        public float DamageDistance => _damageDistance;
        public LayerMask AttackTargetMask => _attackTargetMask;
        public float SearchAngle => _searchAngle;
        public float SearchDistance => _searchDistance;
        public LayerMask SearchMask => _searchMask;

  
    }
}
