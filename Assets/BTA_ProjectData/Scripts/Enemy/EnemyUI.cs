using System;
using UI;
using UnityEngine;

namespace Enemy
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _uiCanvas;
        [SerializeField]
        private HealthBarUI _healthBar;
        
        private Camera _camera;
        private bool _isInitialize;

        public void InitUI(Camera camera, float maxHealth)
        {
            _isInitialize = true;

            _camera = camera;

            _healthBar.InitUI(maxHealth);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }


        public void ChangeHealth(float value)
        {
            _healthBar.ChangeHealth(value);
        }

        private void LateUpdate()
        {
            if (_isInitialize == false)
                return;

            var lookPos = transform.position - _camera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
}
