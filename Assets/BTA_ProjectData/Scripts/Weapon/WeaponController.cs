using Abstraction;
using BTAPlayer;
using Configs;
using Photon.Pun;
using System;
using System.Collections;
using Tools;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : MonoBehaviour , IPaused
    {
        [SerializeField]
        private Transform _cameraPos;
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
            Subscribe();

            _isInitialize = true;
        }

        private void Subscribe()
        {
            PlayerInput.OnShootInput += Shoot;
            PlayerInput.OnReloadInput += StartReload;

            GameStateManager.OnGamePaused += OnPause;
        }

        private void Unubscribe()
        {
            PlayerInput.OnShootInput -= Shoot;
            PlayerInput.OnReloadInput -= StartReload;

            GameStateManager.OnGamePaused -= OnPause;
        }

        private void Update()
        {
            if (!_isInitialize)
                return;
            if (_isPaused)
                return;

            _timeSinceLastShoot += Time.deltaTime;

#if UNITY_EDITOR
            Debug.DrawRay(_muzzle.position, _muzzle.forward * _data.MaxDistance);
#endif
        }

        private void Shoot()
        {
            if (_isPaused)
                return;

            if (_currentAmmo > 0)
            {
                if (CanShoot())
                {
                    if (Physics.Raycast(_cameraPos.position, _cameraPos.forward, out RaycastHit hitInfo, _data.MaxDistance))
                    {
                        var damageable = hitInfo.transform.GetComponent<IDamageable>();
                        damageable?.TakeDamage(new DamageData 
                        {
                            Value = _data.Damage,
                            Force = -hitInfo.normal * _data.DamageForce
                        });
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
            if (_isPaused)
                return;

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
            Unubscribe();
        }

        #region IPaused

        private bool _isPaused;

        public void OnPause(bool state)
        {
            _isPaused = state;
        }

        #endregion
    }
}
