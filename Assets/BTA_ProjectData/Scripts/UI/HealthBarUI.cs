using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField]
        private Image _healthBarSprite;
        [SerializeField]
        private float _reduceHealthSpeed = 2f;

        private float _maxHealth;

        private float _targetHealthValue;

        public void InitUI(float maxHealth)
        {
            _maxHealth = maxHealth;
            ChangeHealth(maxHealth);
        }

        public void ChangeHealth(float value)
        {
            _targetHealthValue = value / _maxHealth;
        }

        private void Update()
        {
            _healthBarSprite.fillAmount
              = Mathf.MoveTowards(_healthBarSprite.fillAmount, _targetHealthValue, _reduceHealthSpeed);
        }
    }
}
