using Abstraction;
using Enumerators;
using System;

namespace Prefs
{
    public class GameLobbyPrefs
    {
        private GameLobbyState _lobbyState;

        private bool _isNeedRoomCreation;
        private string _roomName;
        private byte _roomMaxPlayers;
        
        public bool IsNeedRoomCreation => _isNeedRoomCreation;
        public string RoomName => _roomName;
        public byte RoomMaxPlayers => _roomMaxPlayers;


        public event Action<GameLobbyState> OnStateChange;

        
        public void ChangeState(GameLobbyState state)
        {
            _lobbyState = state;

            OnStateChange?.Invoke(_lobbyState);
        }

        public void SetRoomData(CreationRoomData data, bool isNeedCreation = false)
        {
            _roomName = data.RoomName;
            _roomMaxPlayers = data.MaxPlayers;

            _isNeedRoomCreation = isNeedCreation;
        }
    }
}
