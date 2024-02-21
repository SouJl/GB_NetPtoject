using UnityEngine;
using TMPro;
namespace UI
{
    public class PlayerDeadViewUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _playerState;


        public void ChangeState(string playerState)
        {
            _playerState.text = $"{playerState}...";
        }

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
