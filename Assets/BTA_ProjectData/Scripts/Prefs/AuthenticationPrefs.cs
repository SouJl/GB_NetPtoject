using Enumerators;
using System;

namespace Prefs
{
    public class AuthenticationPrefs
    {
        private AuthenticationState _state;

        public event Action<AuthenticationState> OnStateChange;

        public AuthenticationPrefs()
        {
            _state = AuthenticationState.None;
        }

        public void ChangeState(AuthenticationState state)
        {
            _state = state;

            OnStateChange?.Invoke(state);
        }
    }
}
