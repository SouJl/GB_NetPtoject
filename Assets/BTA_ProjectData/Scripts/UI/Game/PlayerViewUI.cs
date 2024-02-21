﻿using UnityEngine;
using TMPro;

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

        public void InitUI(string name, float maxHealth, int weaponMagSize)
        {
            _playerName.text = name;
            
            _playerLevel.text = "";

            _weaponMagSize.text = weaponMagSize.ToString();

            ChangeCurrentAmmo(weaponMagSize);

            _healthBar.InitUI(maxHealth);

            gameObject.SetActive(true);
        }

        public void ChangeHealth(float value)
        {
            _healthBar.ChangeHealth(value);
        }

        public void ChangePlayerLevel(int value)
        {
            _playerLevel.text = $"Current level: {value}";
        }

        public void ChangeCurrentAmmo(int value)
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