using Abstraction;
using Configs;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyController : MonoBehaviourPunCallbacks, IDamageable
    {
        [Header("Enemy Data")]
        [SerializeField]
        private EnemyConfig _config;
        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private Transform _selfTransform;
        [SerializeField]
        private Transform _pointOfView;
        [SerializeField]
        private TargetSearchComponent _targetSearch;
        [SerializeField]
        private EnemyUI _unitUI;
        [SerializeField]
        private ParticleSystem _deathEffecet;
  
        private Vector3 _currentTargetPos;
        private Vector3[] _patrollPoints;
        private int _currentPatrolIndex = 0;

        private Camera _mainCamera;
        private Rigidbody _rigidBody;

        private float _currentHealth;
        private EnemyState _currentState;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;
                _unitUI.ChangeHealth(value);
            }
        }

        public EnemyState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
            }
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _agent.speed = _config.MoveSpeed;

            _unitUI.InitUI(_mainCamera, _config.MaxHealth);

            _currentHealth = _config.MaxHealth;

            var photonView = GetComponent<PhotonView>();

            _patrollPoints = new Vector3[photonView.InstantiationData.Length];

            for (int i = 0; i < photonView.InstantiationData.Length; i++)
            {
                _patrollPoints[i] = (Vector3)photonView.InstantiationData[i];
            }

            ChangeState(EnemyState.Patrol);
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;

            UpdateBehavior(Time.deltaTime);
        }

        private void UpdateBehavior(float deltaTime)
        {
            CheckForTarget();

            switch (_currentState)
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
                case EnemyState.Dead:
                    {
                        ExecuteDeadState(deltaTime);
                        break;
                    }
            }
        }


        private void CheckForTarget()
        {
            if (_currentState == EnemyState.Dead)
                return;

            var findResults = Physics.OverlapSphere(_pointOfView.position, _config.SearchDistance, _config.SearchMask);

            if (findResults.Length != 0)
            {
                for (int i = 0; i < findResults.Length; i++)
                {
                    var target = findResults[i].gameObject.GetComponentInParent<IFindable>();
                    if (target != null)
                    {
                        if (_targetSearch.IsVisible(target, _pointOfView, _config.SearchAngle, _config.SearchDistance, _config.SearchMask) == true)
                        {
                            photonView.RPC(nameof(UpdateTargetOnClient), RpcTarget.AllViaServer, new object[] { photonView.ViewID, target.Transform.position });

                            ChangeState(EnemyState.MoveToTarget);
                        }
                    }
                }
            }
        }


        private void ExecuteIdleState()
        {

        }

        private void ExecutePatrolState()
        {

            if (_agent.isActiveAndEnabled && _agent.remainingDistance < 0.1f)
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrollPoints.Length;

                _currentTargetPos = _patrollPoints[_currentPatrolIndex];

                _agent.SetDestination(_currentTargetPos);
            }
        }

        private void ExecuteMoveToTargetState()
        {
            if (_agent.isActiveAndEnabled && _agent.remainingDistance < 0.1f)
            {
                Debug.Log("Reached target position");
            }
        }

        private float _deadDelayProgress;

        private void ExecuteDeadState(float deltaTime)
        {
            _deadDelayProgress += deltaTime;

            if (_deadDelayProgress > _config.DeadDelay)
            {
                _deadDelayProgress = 0;

                PhotonNetwork.Destroy(gameObject);

                Instantiate(_deathEffecet, transform.position, transform.rotation);

               /* photonView.RPC(nameof(DestroyOnClient), RpcTarget.Others, new object[] { photonView.ViewID });
                
                Destroy(gameObject);*/
            }
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
                        _currentTargetPos = _patrollPoints[0];
                        _agent.SetDestination(_currentTargetPos);
                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        _agent.SetDestination(_currentTargetPos);
                        break;
                    }
                case EnemyState.Dead:
                    {
                        _agent.isStopped = true;

                        _agent.ResetPath();

                        _deadDelayProgress = 0;
                        break;
                    }
            }

            CurrentState = newState;

            photonView.RPC(nameof(UpdateStateOnClient), RpcTarget.Others, new object[] { photonView.ViewID, CurrentState });
        }


        public void TakeDamage(DamageData damage)
        {
            if (CurrentState == EnemyState.Dead)
                return;

            CurrentHealth -= damage.Value;

            if (CurrentHealth <= 0)
            {
                ChangeState(EnemyState.Dead);
            }

            photonView.RPC(nameof(UpdateHealthOnClient), RpcTarget.Others, new object[] { photonView.ViewID, CurrentHealth });
        }


        #region PUNRPC_METHODS

        [PunRPC]
        private void UpdateHealthOnClient(int id, float value)
        {
            if (photonView.ViewID != id)
                return;

            CurrentHealth = value;
        }

        [PunRPC]
        public void UpdateStateOnClient(int id, EnemyState value)
        {
            if (photonView.ViewID != id)
                return;



            CurrentState = value;
        }

        [PunRPC]
        private void UpdateTargetOnClient(int id, Vector3 position)
        {
            if (photonView.ViewID != id)
                return;

            _currentTargetPos = position;
        }

        [PunRPC]
        private void DestroyOnClient(int id)
        {
            if (photonView.ViewID != id)
                return;

            Destroy(gameObject);
        }

        #endregion


        #region OnDraw In Editor

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_config == null)
                return;

            Handles.color = Color.black;
            Handles.DrawWireDisc(_pointOfView.position, Vector3.up, _config.SearchDistance);

            Handles.color = Color.yellow;
            Handles.DrawWireArc(_pointOfView.position, Vector3.up, transform.forward, _config.SearchAngle / 2, _config.SearchDistance);
            Handles.DrawWireArc(_pointOfView.position, Vector3.up, transform.forward, -_config.SearchAngle / 2, _config.SearchDistance);

            Vector3 viewAngle01 = DirectionFromAngle(_pointOfView.eulerAngles.y, -_config.SearchAngle / 2);
            Vector3 viewAngle02 = DirectionFromAngle(_pointOfView.eulerAngles.y, _config.SearchAngle / 2);

            Handles.color = Color.yellow;
            Handles.DrawLine(_pointOfView.position, _pointOfView.position + viewAngle01 * _config.SearchDistance);
            Handles.DrawLine(_pointOfView.position, _pointOfView.position + viewAngle02 * _config.SearchDistance);
        }


        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
#endif

        #endregion
    }
}
