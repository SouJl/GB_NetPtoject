using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverScreenUI : MonoBehaviour
    {
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private Button _mainMenuButton;
        [SerializeField]
        private Button _exitGameButton;

        public Button RestartButton => _restartButton;
        public Button MainMenuButton => _mainMenuButton;
        public Button ExitGameButton => _exitGameButton;

        public void Show()
{
            gameObject.SetActive(true);
        }

        public void Hide()
{
            gameObject.SetActive(false);
        }
    }
}
