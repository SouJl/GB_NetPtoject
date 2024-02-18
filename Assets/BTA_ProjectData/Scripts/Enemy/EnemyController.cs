using Abstraction;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Stats")]
        [SerializeField]
        private float _maxHealth = 100f;

        [Header("Base Settings")]
        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private Transform _selfTransform;
        [SerializeField]
        private Transform _pointOfView;
        [SerializeField]
        private EnemyUI _unitUI;

        [Header("Find Target Settings")]
        [SerializeField]
        private float _angle = 90f;
        [SerializeField]
        private float _searchDistance = 10f;
        [SerializeField]
        private LayerMask _targetMask;
        [SerializeField]
        private TargetSearchComponent _targetSearch;


        [Header("Patroll Settings")]
        [SerializeField]
        private Transform[] _patrollPoints;


        private Camera _mainCamera;
        private float _currentHealth;
        private EnemyState _state;

        private int _currentPatrolIndex = 0;

        private Transform _currentTarget;

        [HideInInspector]
        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;
                _unitUI.ChangeHealth(value);
            }
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            _unitUI.InitUI(_mainCamera, _maxHealth);

            _currentHealth = _maxHealth;

            ChangeState(EnemyState.Patrol);
        }

        private void Update()
        {
            UpdateBehavior();
        }

        private void UpdateBehavior()
        {
            switch (_state)
            {
                case EnemyState.Idle:
                    {
                        ExecuteIdleState();
                        break;
                    }
                case EnemyState.Patrol:
                    {
                        ExecutePatrolState();
                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        ExecuteMoveToTargetState();
                        break;
                    }
            }
        }

        private void CheckForTarget()
        {
            if (Physics.Raycast(_pointOfView.position, _pointOfView.forward, out var hit, _searchDistance, _targetMask))
            {
                var target = hit.collider.gameObject.GetComponentInParent<IFindable>();
                if (target != null)
                {
                    if(_targetSearch.IsVisible(target, _pointOfView, _angle, _searchDistance, _targetMask) == true)
                    {
                        _currentTarget = target.Transform;
                        ChangeState(EnemyState.MoveToTarget);
                    }
                }
            }
        }

        private void ExecuteIdleState()
        {

        }

        private void ExecutePatrolState()
        {
            if(_agent.remainingDistance < 0.1f)
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrollPoints.Length;

                var patrolPoint = _patrollPoints[_currentPatrolIndex].position;

                _agent.SetDestination(patrolPoint);
            }
        }

        private void ExecuteMoveToTargetState()
        {

        }


        private void ChangeState(EnemyState newState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    {
                        break;
                    }
                case EnemyState.Patrol:
                    {
                        var startPoint = _patrollPoints[0];
                        _agent.SetDestination(startPoint.position);
                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        break;
                    }
            }

            _state = newState;
        }

        public void TakeDamage(float damageValue)
        {
            if (CurrentHealth > 0)
            {
                CurrentHealth -= damageValue;
            }
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
