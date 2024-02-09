using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BTAPlayer
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _uiCanvas;
        [SerializeField]
        private Transform _orientation;
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private Image _healthBarSprite;
        [SerializeField]
        private float _reduceHealthSpeed = 2f;

        private Camera _camera;
        private float _maxHealth;
        
        private float _targetHealthValue;

        public void Init(Camera camera, string name, float maxHealth)
        {
            _camera = camera;

            _uiCanvas.worldCamera = _camera;

            SetName(name);

            _maxHealth = maxHealth;
            ChangeHealth(_maxHealth);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetName(string name)
        {
            _name.text = name;
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

        private void LateUpdate()
        {
            var lookPos = transform.position - _camera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

}
