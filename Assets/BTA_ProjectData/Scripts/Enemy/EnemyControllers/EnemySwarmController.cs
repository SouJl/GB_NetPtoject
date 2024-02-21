using Abstraction;
using Configs;
using Enumerators;
using Photon.Pun;
using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySwarmController : EnemyBaseController
    {

        [Header("Enemy Swarm Settings")]
        [SerializeField]
        private EnemyConfig _config;

        private float _currentHealth;
        private EnemyState _currentState;

        private bool _isPlayerFound;
        private bool _isPatroling;

        private Vector3 _spawnCenter;
        private float _patrolRadius;

        private Transform _currentTarget;
        private Vector3 _currentTargetPos;

        public override EnemyType Type => EnemyType.Swarm;
        
        public override event Action<EnemyBaseController> OnDestroy;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;
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

        public event Action<Transform> OnSetNewTarget;
        public event Action<EnemySwarmController> OnRemoved;

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();

            _agent.speed = _config.MoveSpeed;
            _currentHealth = _config.MaxHealth;

            _isPlayerFound = false;

            var photonView = GetComponent<PhotonView>();

            var instData = photonView.InstantiationData;

            _spawnCenter = (Vector3)instData[0];
            _patrolRadius = (float)instData[1];

            _currentTargetPos = transform.position;

            ChangeState(EnemyState.Idle);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (!photonView.IsMine)
                return;

            UpdateTargetPos();

            CheckForTarget();

            switch (_currentState)
            {
                case EnemyState.Idle:
                    {
                        var newPatrolPos = SelectNewPatrolPos();

                        photonView.RPC(nameof(UpdateTargetOnClient), RpcTarget.AllViaServer, new object[] { photonView.ViewID, newPatrolPos });

                        ChangeState(EnemyState.Patrol);

                        break;
                    }
                case EnemyState.Patrol:
                    {
                        if (_agent.isActiveAndEnabled && _agent.remainingDistance < 0.1f)
                        {
                            _currentTargetPos = SelectNewPatrolPos();

                            photonView.RPC(nameof(UpdateTargetOnClient), RpcTarget.Others, new object[] { photonView.ViewID, _currentTargetPos });

                            _agent.SetDestination(_currentTargetPos);
                        }

                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        if (_agent.isActiveAndEnabled && _agent.remainingDistance < _config.DamageDistance / 2f)
                        {
                            ChangeState(EnemyState.Attack);
                        }
                        break;
                    }
                case EnemyState.Attack:
                    {
                        var findResults = Physics.OverlapSphere(_pointOfView.position, _config.DamageDistance, _config.AttackTargetMask);

                        if (findResults.Length != 0)
                        {
                            for (int i = 0; i < findResults.Length; i++)
                            {
                                var target = findResults[i].gameObject.GetComponentInParent<IDamageable>();

                                if (target != null)
                                {
                                    target.TakeDamage(new DamageData
                                    {
                                        Value = _config.DamageValue,
                                    });
                                }
                            }

                            ChangeState(EnemyState.Dead);
                        }
                        else
                        {
                            ChangeState(EnemyState.Idle);
                        }

                        break;
                    }
                case EnemyState.Dead:
                    {
                        OnRemoved?.Invoke(this);

                        PhotonNetwork.InstantiateRoomObject($"Effects/{_deathEffecet.name}", transform.position, transform.rotation);

                        PhotonNetwork.Destroy(gameObject);

                        break;
                    }
            }
        }

        private void UpdateTargetPos()
        {
            if (CurrentState != EnemyState.MoveToTarget)
                return;
            if (_currentTarget == null)
                return;

            _currentTargetPos = _currentTarget.position;

            photonView.RPC(nameof(UpdateTargetOnClient), RpcTarget.Others, new object[] { photonView.ViewID, _currentTargetPos });
        }


        private void CheckForTarget()
        {
            switch (CurrentState)
            {
                case EnemyState.Dead:
                case EnemyState.MoveToTarget:
                case EnemyState.Attack:
                    {
                        return;
                    }
            }

            var findResults = Physics.OverlapSphere(_pointOfView.position, _config.SearchDistance, _config.SearchMask);

            if (findResults.Length != 0)
            {
                for (int i = 0; i < findResults.Length; i++)
                {
                    var target = findResults[i].gameObject.GetComponentInParent<IFindable>();
                    if (target != null)
                    {
                        OnSetNewTarget?.Invoke(target.Transform);
                    }
                }
            }
        }

        private Vector3 SelectNewPatrolPos()
        {
            return _spawnCenter + Random.insideUnitSphere * _patrolRadius;
        }

        private void ChangeState(EnemyState newState)
        {
            Debug.Log(newState);
            switch (newState)
            {
                case EnemyState.Idle:
                    {
                        _currentTarget = null;
                        break;
                    }
                case EnemyState.Patrol:
                    {
                        _currentTarget = null;

                        _agent.isStopped = false;

                        _agent.ResetPath();

                        _agent.SetDestination(_currentTargetPos);
                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        _agent.isStopped = false;

                        _agent.ResetPath();

                        _agent.SetDestination(_currentTargetPos);
                        break;
                    }
                case EnemyState.Attack:
                    {
                        _agent.isStopped = true;

                        _agent.ResetPath();

                        break;
                    }
                case EnemyState.Dead:
                    {
                        _currentTarget = null;

                        _agent.isStopped = true;

                        _agent.ResetPath();

                        break;
                    }
            }

            CurrentState = newState;

            photonView.RPC(nameof(UpdateStateOnClient), RpcTarget.Others, new object[] { photonView.ViewID, CurrentState });
        }

        public void SetTarget(Transform target)
        {
            if (!photonView.IsMine)
                return;

            _currentTarget = target;

            if (_currentTarget != null)
            {
                _currentTargetPos = _currentTarget.position;

                photonView.RPC(nameof(UpdateTargetOnClient), RpcTarget.Others, new object[] { photonView.ViewID, _currentTargetPos });

                ChangeState(EnemyState.MoveToTarget);
            }
        }

        public override void TakeDamage(DamageData damage)
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
            Handles.DrawWireDisc(_pointOfView.position, Vector3.up, _config.DamageDistance);
        }
#endif

        #endregion
    }
}
