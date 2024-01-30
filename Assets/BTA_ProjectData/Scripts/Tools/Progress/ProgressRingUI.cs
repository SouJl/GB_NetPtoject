using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class ProgressRingUI : MonoBehaviour
    {
        [SerializeField]
        private Slider _progressSlider;
        [SerializeField]
        private float _minValue = 0f;
        [SerializeField]
        private float _maxValue = 1f;
        [SerializeField]
        private float _speed = 1f;

        public float MinValue => _minValue;
        public float MaxValue => _maxValue;
        public float Speed => _speed;

        public void InitUI()
        {
            _progressSlider.minValue = _minValue;
            _progressSlider.maxValue = _maxValue;

            _progressSlider.value = 0;

            Hide();
        }

        public void UpdateProgressValue(float value)
        {
            _progressSlider.value = value;
        }

        public void Show()
            => gameObject.SetActive(true);
        public void Hide()
            => gameObject.SetActive(false);

    }
}

