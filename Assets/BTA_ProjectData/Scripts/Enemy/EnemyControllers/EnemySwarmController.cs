using Abstraction;
using Configs;
using Enumerators;
using Photon.Pun;
using System;
using System.Collections.Generic;
using Tools;
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
        [SerializeField]
        private EnemyState _currentState;

        private bool _isPlayerFound;
        private bool _isPatroling;

        public override EnemyType Type => EnemyType.Swarm;
        
        public override event Action<EnemyBaseController, string> OnDestroy;

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

        protected override void OnAwake()
        {
            base.OnAwake();

            _agent.speed = _config.MoveSpeed;
            _currentHealth = _config.MaxHealth;

            ChangeState(EnemyState.Idle);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (!photonView.IsMine)
                return;
            
            if (GameStateManager.Players.Count == 0)
                return;

            UpdateTartgetState();

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    {
                        SetTarget();
                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        var players = GameStateManager.Players;

                        var targetPos = players[_targerIndex].GetPosition();

                        var dist = Vector3.Distance(transform.position, targetPos);

                        if ( dist < _config.DamageDistance * 0.8f)
                        {
                            ChangeState(EnemyState.Attack);
                        }
                        else
                        {
                            _agent.SetDestination(targetPos);
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

                                    OnDestroy?.Invoke(this, string.Empty);

                                    ChangeState(EnemyState.Dead);

                                    return;
                                }
                            }
                        }
                        else
                        {
                            ChangeState(EnemyState.MoveToTarget);
                        }

                        break;
                    }
                case EnemyState.Dead:
                    {
                        PhotonNetwork.InstantiateRoomObject($"Effects/{_deathEffecet.name}", transform.position, transform.rotation);

                        PhotonNetwork.Destroy(gameObject);

                        break;
                    }
            }
        }

        private void UpdateTartgetState()
        {
            var players = GameStateManager.Players;

            if(players[_targerIndex].IsAvailable == false)
            {
                var resultIndex = -1;

                var availableTargetsId = new List<int>();

                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].IsAvailable)
                    {
                        availableTargetsId.Add(i);
                    }
                }

                if (availableTargetsId.Count != 0)
                    resultIndex = availableTargetsId[Random.Range(0, availableTargetsId.Count)];

                if (resultIndex < 0)
                {
                    ChangeState(EnemyState.None);
                }
                else
                {
                    _targerIndex = resultIndex;
                }
            }
        }

        private int _targerIndex;
        private List<IFindable> _targets;

        public void SetTarget()
        {
            if (!photonView.IsMine)
                return;

            _targerIndex = Random.Range(0, GameStateManager.Players.Count);

            var currentPos = GameStateManager.Players[_targerIndex].GetPosition();

            _agent.SetDestination(currentPos);


            ChangeState(EnemyState.MoveToTarget);
        }

        private void ChangeState(EnemyState newState)
        {
            Debug.Log(newState);

            switch (newState)
            {
                default:
                    {
                        _agent.isStopped = true;

                        _agent.ResetPath();

                        _rigidBody.velocity = Vector3.zero;
                        break;
                    }
                case EnemyState.Idle:
                    {
                        _agent.isStopped = true;

                        _agent.ResetPath();
                        break;
                    }
                case EnemyState.MoveToTarget:
                    {
                        _agent.isStopped = false;

                        _agent.ResetPath();

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
                        _agent.isStopped = true;

                        _agent.ResetPath();

                        break;
                    }
            }

            photonView.RPC(nameof(UpdateStateOnClient), RpcTarget.AllViaServer, new object[] { photonView.ViewID, newState });
        }

        public override void TakeDamage(DamageData damage)
        {
            if (CurrentState == EnemyState.Dead)
                return;

            CurrentHealth -= damage.Value;

            photonView.RPC(nameof(UpdateHealthOnClient), RpcTarget.AllViaServer, new object[] { photonView.ViewID, CurrentHealth, damage.HolderId });
        }

        #region PUNRPC_METHODS

        [PunRPC]
        private void UpdateHealthOnClient(int id, float value, string dealerId)
        {
            if (photonView.ViewID != id)
                return;

            CurrentHealth = value;

            if (CurrentHealth <= 0)
            {

                OnDestroy?.Invoke(this, dealerId);

                ChangeState(EnemyState.Dead);
            }
        }

        [PunRPC]
        public void UpdateStateOnClient(int id, EnemyState value)
        {
            if (photonView.ViewID != id)
                return;

            CurrentState = value;
        }

        [PunRPC]
        private void DestroyOnClient(int id)
        {
            if (photonView.ViewID != id)
                return;

            Destroy(gameObject);
        }

        [PunRPC]
        private void SetAgentDistanation(int id, Vector3 value)
        {
            if (photonView.ViewID != id)
                return;

            _agent.SetDestination(value);
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
