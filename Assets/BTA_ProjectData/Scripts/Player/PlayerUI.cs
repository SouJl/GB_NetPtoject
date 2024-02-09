﻿using UnityEngine;
using TMPro;
using UI;

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
        private HealthBarUI _healthBar;

        private Camera _camera;

        public void Init(Camera camera, string name, float maxHealth)
        {
            _camera = camera;

            _uiCanvas.worldCamera = _camera;

            SetName(name);

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

        public void SetName(string name)
        {
            _name.text = name;
        }

        public void ChangeHealth(float value)
        {
            _healthBar.ChangeHealth(value);
        }

        private void LateUpdate()
        {
            var lookPos = transform.position - _camera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

}
