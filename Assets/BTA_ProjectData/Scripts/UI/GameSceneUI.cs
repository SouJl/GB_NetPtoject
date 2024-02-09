using UnityEngine;
using TMPro;

namespace UI
{
    public class GameSceneUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _playerName;
        [SerializeField]
        private HealthBarUI _healthBar;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void InitUI(string name, float maxHealth)
        {
            _playerName.text = name;

            _healthBar.InitUI(maxHealth);

            gameObject.SetActive(true);
        }

        public void ChangeHealth(float value)
        {
            _healthBar.ChangeHealth(value);
        }
    }
}
