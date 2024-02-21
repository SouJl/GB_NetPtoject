using System;
using UnityEngine;

namespace Tools
{
    public class GameStateManager : MonoBehaviour
    {
        public static event Action<bool> OnGamePaused;

        public static void ChangePauseGameState(bool state)
        {
            OnGamePaused?.Invoke(state);
        }

    }
}
