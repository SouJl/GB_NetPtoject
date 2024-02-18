using Photon.Pun;
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
    }
}
