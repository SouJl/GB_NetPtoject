using Abstraction;
using Enumerators;
using System;

namespace Prefs
{
    public class GameLobbyPrefs
    {
        private GameLobbyState _lobbyState;

        private string _nickName;
        private bool _isNeedRoomCreation;
        private CreationRoomData _creationData;

        public string NickName => _nickName;
        public bool IsNeedRoomCreation => _isNeedRoomCreation;
        public CreationRoomData CreationData => _creationData;

        public event Action<GameLobbyState> OnStateChange;
        
        public GameLobbyPrefs(string userNickName)
        {
            _nickName = userNickName;
        }

        public void ChangeState(GameLobbyState state)
        {
            _lobbyState = state;

            OnStateChange?.Invoke(_lobbyState);
        }

        public void SetRoomData(CreationRoomData data, bool isNeedCreation = false)
        {
            _creationData = data;

            _isNeedRoomCreation = isNeedCreation;
        }
    }
}
