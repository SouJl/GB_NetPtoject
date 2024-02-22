using Abstraction;
using Enumerators;
using System;
using UnityEngine;

namespace Prefs
{
    public class AuthenticationPrefs
    {
        private AuthenticationState _state;
        public event Action<AuthenticationState> OnStateChange;

        public IGameUser UserData { get; private set; }

        public IGamePlayer PlayerData { get; private set; }
        
        public AudioSource ClickSound { get; set; }

        public AuthenticationPrefs()
        {
            _state = AuthenticationState.None;

            UserData = new BaseGameUser();
            PlayerData = new BaseGamePlayer();
        }

        public void ChangeState(AuthenticationState state)
        {
            _state = state;

            OnStateChange?.Invoke(_state);
        }

        public void SetUserdata(IGameUser userData)
        {
            UserData = userData;
        }

        public void SetPlayerData(IGamePlayer playerdata)
        {
            PlayerData = playerdata;
        }

        public void Clear()
        {
            UserData = null;
            PlayerData = null;
        }
    }
}
