using UnityEngine;
using TMPro;

namespace GameLobby
{
    public class PlayerInfoObjectUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _playerName;

        public void InitUI(string playerName)
        {
            _playerName.text = playerName;
        }
    }
}
