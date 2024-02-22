using Abstraction;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class GameStateManager : MonoBehaviour
    {
        public static event Action<bool> OnGamePaused;
        public static event Action OnGameOver;


        public static List<IFindable> Players = new List<IFindable>(5);

        public static void ChangePauseGameState(bool state)
        {
            OnGamePaused?.Invoke(state);
        }

        public static void AddPlayer(IFindable player)
        {
            Players.Add(player);
        }

        public static void GameOver()
        {
            OnGameOver?.Invoke();
        }
    }
}
