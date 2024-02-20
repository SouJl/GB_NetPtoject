using BTAPlayer;
using Configs;
using Enumerators;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawnController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private bool _showEnemySpawnGUI;

        [Header("Enemy Prefabs")]
        [SerializeField]
        private GameObject _enemyGunnerPrefab;
        [SerializeField]
        private GameObject _enemySwarmPrefab;
        [SerializeField]
        private GameObject _enemySniperPrefab;

        [Space(10), Header("Wave Sttings")]
        [SerializeField]
        private WaveList _waves;

        [Space(10), Header("Spawn Sttings")]
        [SerializeField]
        private float _spawnTime = 3f;
        [SerializeField]
        private GameObject _spawnEffect;
        [SerializeField]
        private float _swarmSpawnRadius;
        [SerializeField]
        private float _swarmPatrolRadius;
        [SerializeField]
        private List<Transform> _gunnerSpawnPoints;
        [SerializeField]
        private List<Transform> _swarmSpawnPoints;
        [SerializeField]
        private List<Transform> _sniperSpawnPoints;


        private List<IPlayerController> _availablePlayers = new();

        public void AddPlayer(IPlayerController player)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            _availablePlayers.Add(player);
        }

        public void StartEnemySpawn()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            for (int i = 0; i < _waves.Collection.Count; i++)
            {
                var currentWave = _waves.Collection[i];

                yield return new WaitForSeconds(currentWave.StartDelayTime);

                StartCoroutine(SpawnWave(currentWave));

                yield return new WaitForSeconds(currentWave.Wavetime);
            }
        }

        private IEnumerator SpawnWave(WaveConfig wave)
        {
            for (int i = 0; i < wave.Enemies.Length; i++)
            {
                var enemyWave = wave.Enemies[i];

                Transform spawnPoint = null;

                switch (enemyWave.Type)
                {
                    case EnemyType.Gunner:
                        {
                            var spawnPoints = new Transform[enemyWave.Count];

                            for (int j = 0; j < enemyWave.Count; j++)
                            {
                                var index = Random.Range(0, _gunnerSpawnPoints.Count);
                                spawnPoints[j] = _gunnerSpawnPoints[index];
                            }

                            for (int j = 0; j < spawnPoints.Length; j++)
                            {
                                InstantiateSpawnEffect(spawnPoints[j]);
                            }

                            yield return new WaitForSeconds(_spawnTime);

                            for (int j = 0; j < spawnPoints.Length; j++)
                            {
                                SpawnGunner(spawnPoints[j]);
                            }

                            break;
                        }
                    case EnemyType.Swarm:
                        {
                            var index = Random.Range(0, _swarmSpawnPoints.Count);
                            spawnPoint = _gunnerSpawnPoints[index];
                            break;
                        }
                    case EnemyType.Sniper:
                        {
                            var index = Random.Range(0, _sniperSpawnPoints.Count);
                            spawnPoint = _gunnerSpawnPoints[index];
                            break;
                        }
                }
            }
        }

        private void InstantiateSpawnEffect(Transform InstantiatePoint)
        {
            PhotonNetwork.InstantiateRoomObject($"Effects/{_spawnEffect.name}", InstantiatePoint.position, Quaternion.identity);
        }

        private void SpawnGunner(Transform spawnPoint)
        {
            PhotonNetwork.Instantiate(_enemyGunnerPrefab.name, spawnPoint.position, Quaternion.identity, 0, new object[] { });
        }

        private void SpawnSwarm(Transform spawnPoint)
        {
            var randomSpawnPos = Random.insideUnitCircle * _swarmSpawnRadius;

            var spawnPos = spawnPoint.position + new Vector3(randomSpawnPos.x, 0, randomSpawnPos.y);

            PhotonNetwork.Instantiate(_enemySwarmPrefab.name, spawnPos, Quaternion.identity, 0, new object[] { });
        }


#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!_showEnemySpawnGUI)
                return;

            if (_gunnerSpawnPoints.Count == 0)
                return;

            for (int i = 0; i < _gunnerSpawnPoints.Count; i++)
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(_gunnerSpawnPoints[i].position, Vector3.up, 0.3f);
            }


            if (_swarmSpawnPoints.Count == 0)
                return;

            for (int i = 0; i < _swarmSpawnPoints.Count; i++)
            {
                Handles.color = Color.magenta;
                Handles.DrawWireDisc(_swarmSpawnPoints[i].position, Vector3.up, _swarmSpawnRadius);
            }


        }
#endif
    }
}
