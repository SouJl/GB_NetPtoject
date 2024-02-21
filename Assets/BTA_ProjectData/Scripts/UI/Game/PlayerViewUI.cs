using UnityEngine;
using TMPro;
using BTAPlayer;
using System;

namespace UI
{
    public class PlayerViewUI : MonoBehaviour
    {
        [Header("Player Stats Settings")]
        [SerializeField]
        private TMP_Text _playerName;
        [SerializeField]
        private TMP_Text _playerLevel;
        [SerializeField]
        private HealthBarUI _healthBar;

        [Header("Weapon UI Settings")]
        [SerializeField]
        private TMP_Text _weaponMagSize;
        [SerializeField]
        private TMP_Text _weapomCurrentAmmo;

        private void Awake()
        {
            PlayerInput.OnNameChanged += ChangeName;
            PlayerInput.OnHealthChanged += ChangeHealth;
            PlayerInput.OnLevelChanged += ChangeLevel;
            PlayerInput.OnAmmoChanged += ChangeAmmo;
        }


        public void InitUI(string name, float maxHealth, int weaponMagSize)
        {
            _playerName.text = name;
            
            _playerLevel.text = "";

            _weaponMagSize.text = weaponMagSize.ToString();

            ChangeAmmo(weaponMagSize);

            _healthBar.InitUI(maxHealth);

            gameObject.SetActive(true);
        }

        private void ChangeName(string name)
        {
            _playerName.text = name;
        }

        public void ChangeHealth(float value)
        {
            _healthBar.ChangeHealth(value);
        }

        public void ChangeLevel(int value)
        {
            _playerLevel.text = $"Current level: {value}";
        }

        public void ChangeAmmo(int value)
        {
            _weapomCurrentAmmo.text = value.ToString();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
