using UnityEngine;
using TMPro;
using Tools;
using GameTask;

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

        [Header("Info UI Seconds")]
        [SerializeField]
        private TMP_Text _whaitText;
        [SerializeField]
        private CountDownTimer _timer;
        [SerializeField]
        private TaskManagerView _taskView;

        public void InitUI(string name, float maxHealth, int weaponMagSize)
        {
            _playerName.text = name;
            
            _playerLevel.text = "";

            _weaponMagSize.text = weaponMagSize.ToString();

            ChangeCurrentAmmo(weaponMagSize);

            _healthBar.InitUI(maxHealth);

            gameObject.SetActive(true);

            _whaitText.gameObject.SetActive(false);

            _timer.gameObject.SetActive(false);
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

        public void StartExitCountDown(float time)
        {
            _taskView.gameObject.SetActive(false);

            _whaitText.gameObject.SetActive(true);
            _timer.StartTimer(time);
            
        }

        public void StopExitCountDown() 
            => _timer.StopTimer();
    }
}
