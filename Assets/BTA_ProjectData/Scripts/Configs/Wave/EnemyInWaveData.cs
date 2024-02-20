using Enumerators;
using System;

namespace Configs
{
    [Serializable]
    public class EnemyInWaveData
    {
        public EnemyType Type;
        public int Count;
        public float SpawnTime;
    }
}
