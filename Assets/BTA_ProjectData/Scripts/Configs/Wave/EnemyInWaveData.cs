using Enumerators;
using System;

namespace Configs
{
    [Serializable]
    public class EnemyInWaveData
    {
        public EnemyType EnemyType;
        public int Count;
        public float SpawnTime;
    }
}
