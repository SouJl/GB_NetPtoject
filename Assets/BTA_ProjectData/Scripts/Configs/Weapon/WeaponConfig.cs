using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(WeaponConfig), menuName = "B.T.A/" + nameof(WeaponConfig))]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Info")]
        [SerializeField]
        private string _name;

        [Header("Shooting")]
        [SerializeField]
        private float _damage = 10f;
        [SerializeField]
        private float _maxDistance = 100f;

        [Header("Reloading")]
        [SerializeField]
        private int _magSize = 0;
        [Tooltip("In RPM")]
        [SerializeField]
        private float _fireRate = 0f;
        [SerializeField]
        private float _reloadTime = 1.5f;

        public string Name => _name;
        public float Damage => _damage;
        public float MaxDistance => _maxDistance;
        public int MagSize => _magSize;
        public float FireRate => _fireRate;
        public float ReloadTime => _reloadTime;
    }
}
