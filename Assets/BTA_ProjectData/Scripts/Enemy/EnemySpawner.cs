using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviourPun
    {
        [SerializeField]
        private GameObject _enemyPrefab;
        [SerializeField]
        private Transform _enemySpawnPoint;
        [SerializeField]
        private Transform[] _patrolPoints;

        [Header("EnemySwarm Spawn Settings")]
        [SerializeField]
        private bool _showEnemySwarmSpawnGUI;
        [SerializeField]
        private GameObject _swarmPrefab;
        [SerializeField]
        private Transform _swarmSpawnPoint;
        [SerializeField]
        private float _swarmSpawnRadius;
        [SerializeField]
        private float _swarmPatrolRadius;

        public void Spawn()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            var spawnData = new object[_patrolPoints.Length];
            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                spawnData[i] = _patrolPoints[i].position;
            }

            PhotonNetwork.Instantiate(_enemyPrefab.name, _enemySpawnPoint.position, Quaternion.identity, 0, spawnData);
        }

        public void SpawnSwarm()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            var randomSpawnPos = Random.insideUnitCircle * _swarmSpawnRadius;

            var spawnPos = _swarmSpawnPoint.position + new Vector3(randomSpawnPos.x, 0, randomSpawnPos.y);

            PhotonNetwork.Instantiate(_swarmPrefab.name, spawnPos, Quaternion.identity, 0, new object[] { _swarmSpawnPoint.position , _swarmPatrolRadius });
        }


#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!_showEnemySwarmSpawnGUI)
                return;
            if (!_swarmSpawnPoint)
                return;

            Handles.color = Color.red;
            Handles.DrawWireDisc(_swarmSpawnPoint.position, Vector3.up, _swarmSpawnRadius);

            Handles.color = Color.blue;
            Handles.DrawWireDisc(_swarmSpawnPoint.position, Vector3.up, _swarmPatrolRadius);
        }
#endif
    }
}
