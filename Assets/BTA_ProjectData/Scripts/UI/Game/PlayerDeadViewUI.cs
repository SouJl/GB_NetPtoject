using UnityEngine;
using TMPro;
using Tools;

namespace UI
{
    public class PlayerDeadViewUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _playerState;
        [SerializeField]
        private CountDownTimer _timer;
        [SerializeField]
        private float _deadTime = 3f;

        public void ChangeState(string playerState)
        {
            _playerState.text = $"{playerState}...";
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _timer.StartTimer(_deadTime);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _timer.StopTimer();
        }
    }
}
