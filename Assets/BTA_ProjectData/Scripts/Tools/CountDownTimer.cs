using UnityEngine;
using TMPro;

namespace Tools
{
    public class CountDownTimer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _timerText;

        private bool _isEnable;

        private float currentTime;

        public void StartTimer(float time)
        {
            currentTime = time;

            gameObject.SetActive(true);

            _isEnable = true;

            UpdateTimerUI();
        }

        public void StopTimer()
        {
            currentTime = 0f;

            gameObject.SetActive(false);

            _isEnable = false;

            UpdateTimerUI();
        }

        private void Update()
        {
            if (!_isEnable)
                return;

            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                currentTime = 0;
                UpdateTimerUI();
                StopTimer();
            }
        }

        private void UpdateTimerUI()
        {
            int seconds = Mathf.FloorToInt(currentTime % 60);

            _timerText.text = seconds.ToString();
        }
    }
}
