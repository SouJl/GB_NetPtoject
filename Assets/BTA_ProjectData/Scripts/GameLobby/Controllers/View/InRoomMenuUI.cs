using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

namespace GameLobby
{
    public class InRoomMenuUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _roomName;
        [SerializeField]
        private Transform _playersInfoContainer;
        [SerializeField]
        private GameObject _playerInfoPrefab;

        private List<PlayerInfoObjectUI> _playerCollection = new();

        public void InitUI()
        {
            _roomName.text = "";
        }

        public void SetRoomData(Room room)
        {
            _roomName.text = $"ROOM NAME: {room.Name}";
        }

        public void AddPlayer(Player player)
        {
            var playerUI = CreatePlayerInfoView(player);
            _playerCollection.Add(playerUI);
        }

        private PlayerInfoObjectUI CreatePlayerInfoView(Player player)
        {
            GameObject objectView = Instantiate(_playerInfoPrefab, _playersInfoContainer, false);
            var view = objectView.GetComponent<PlayerInfoObjectUI>();

            view.InitUI(player.NickName);

            return view;
        }

        private void OnDestroy()
        {
            for(int i =0; i < _playerCollection.Count; i++)
            {
                var playerInfo = _playerCollection[i];
                Destroy(playerInfo.gameObject);
            }

            _playerCollection.Clear();
        }
    }
}
