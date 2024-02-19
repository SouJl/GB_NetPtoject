using Abstraction;
using BTAPlayer;
using Configs;
using System;
using System.Collections;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : MonoBehaviour
    {

        [SerializeField]
        private Transform _muzzle;
        [SerializeField]
        private ParticleSystem _muzzleEffect;

        private bool _isInitialize = false;

        private WeaponConfig _data;

        private int _currentAmmo;
        private bool _reloading;
        private float _timeSinceLastShoot;

        public int CurrentAmmo
        {
            get => _currentAmmo;
            private set
            {
                _currentAmmo = value;
                OnAmmoChanged?.Invoke(value);
            }
        }

        public event Action<int> OnAmmoChanged;

        public void Init(WeaponConfig data)
        {
            _data = data;

            CurrentAmmo = _data.MagSize;

            PlayerInput.OnShootInput += Shoot;
            PlayerInput.OnReloadInput += StartReload;

            _isInitialize = true;
        }

        private void Update()
        {
            if (!_isInitialize)
                return;

            _timeSinceLastShoot += Time.deltaTime;
            Debug.DrawRay(_muzzle.position, _muzzle.forward * _data.MaxDistance);
        }

        private void Shoot()
        {
            if (_currentAmmo > 0)
            {
                if (CanShoot())
                {
                    if (Physics.Raycast(_muzzle.position, _muzzle.forward, out RaycastHit hitInfo, _data.MaxDistance))
                    {
                        var damageable = hitInfo.transform.GetComponent<IDamageable>();
                        damageable?.TakeDamage(_data.Damage);
                    }

                    CurrentAmmo--;

                    OnAmmoChanged?.Invoke(_currentAmmo);

                    _timeSinceLastShoot = 0;

                    OnGunShoot();
                }
            }
        }

        private bool CanShoot()
        {
            return !_reloading && _timeSinceLastShoot > 1f / (_data.FireRate / 60f);
        }

        private void OnGunShoot()
        {
            _muzzleEffect.Play();
        }

        private void StartReload()
        {
            if (!_reloading && gameObject.activeSelf)
                StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            _reloading = true;

            Debug.Log("Reloading...");

            yield return new WaitForSeconds(_data.ReloadTime);

            CurrentAmmo = _data.MagSize;

            _reloading = false;

            Debug.Log("Reload!");
        }

        private void OnDestroy()
        {
            PlayerInput.OnShootInput -= Shoot;
            PlayerInput.OnReloadInput -= StartReload;
        }
    }
}
