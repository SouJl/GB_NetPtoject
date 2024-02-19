using Abstraction;
using Photon.Pun;
using UnityEngine.AI;
using UnityEngine;

namespace Enemy
{
    public abstract class EnemyBaseController : MonoBehaviourPunCallbacks, IDamageable
    {
        [Header("Enemy Base Settings")]
        [SerializeField]
        protected Transform _selfTransform;
        [SerializeField]
        protected Transform _pointOfView;
        [SerializeField]
        protected NavMeshAgent _agent;
        [SerializeField]
        protected ParticleSystem _deathEffecet;

        protected Camera _mainCamera;
        protected Rigidbody _rigidBody;

        protected float _deltaTime;

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            _mainCamera = Camera.main;
            _rigidBody = GetComponent<Rigidbody>();

            _deltaTime = Time.deltaTime;
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart() 
        {

        }

        private void Update()
        {
            OnUpdate(_deltaTime);
        }

        protected virtual void OnUpdate(float deltaTime)
        {

        }

        public abstract void TakeDamage(DamageData damage);


    }
}
