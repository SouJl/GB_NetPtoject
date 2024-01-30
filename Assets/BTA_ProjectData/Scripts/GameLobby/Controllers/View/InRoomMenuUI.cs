using UnityEngine;
using TMPro;
using Photon.Realtime;

namespace GameLobby
{
    public class InRoomMenuUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _roomName;

        public void InitUI()
        {
            _roomName.text = "";
        }

        public void SetRoomData(Room room)
        {
            _roomName.text = $"WELCOM TO {room.Name}";
        }
    }
}
