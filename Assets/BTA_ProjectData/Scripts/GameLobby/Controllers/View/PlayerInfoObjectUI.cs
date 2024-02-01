using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLobby
{
    public class PlayerInfoObjectUI : MonoBehaviour
    {
        [SerializeField]
        private Image _playerIcon; 
        [SerializeField]
        private TMP_Text _playerName;

        public void InitUI(string playerName, bool isOwner)
        {
            _playerName.text = playerName;

            _playerIcon.color = isOwner ? Color.green : Color.white;
        }
    }
}
