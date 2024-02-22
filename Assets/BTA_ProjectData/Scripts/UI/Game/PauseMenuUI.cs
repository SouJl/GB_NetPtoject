using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button _resumeButton;
        [SerializeField]
        private Button _mainMenuButton;
        [SerializeField]
        private Button _exitGameButton;
        [SerializeField]
        private AudioSource _clickSound;

        public Button ResumeButton => _resumeButton;
        public Button MainMenuButton => _mainMenuButton;
        public Button ExitGameButton => _exitGameButton;

        private void Awake()
        {
            ResumeButton.onClick.AddListener(PlayClick);
            MainMenuButton.onClick.AddListener(PlayClick);
            ExitGameButton.onClick.AddListener(PlayClick);
        }
        private void OnDestroy()
        {
            ResumeButton.onClick.RemoveListener(PlayClick);
            MainMenuButton.onClick.RemoveListener(PlayClick);
            ExitGameButton.onClick.RemoveListener(PlayClick);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void PlayClick()
        {
            _clickSound.Play();
        }

       
    }
}
