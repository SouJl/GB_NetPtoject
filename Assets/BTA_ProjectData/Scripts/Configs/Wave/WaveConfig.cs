using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(WaveConfig), menuName = "B.T.A/" + nameof(WaveConfig))]
    public class WaveConfig :ScriptableObject
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private float _wavetime;
        [SerializeField]
        private float _startDelayTime;
        [SerializeField]
        private EnemyInWaveData[] _enemies;

        public string Name => _name;
        public float StartDelayTime => _startDelayTime;
        public float Wavetime => _wavetime;
        public EnemyInWaveData[] Enemies => _enemies; 
    }
}
