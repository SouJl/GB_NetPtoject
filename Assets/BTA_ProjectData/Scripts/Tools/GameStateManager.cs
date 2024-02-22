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
        public static event Action OnGameWon;

        public static List<IFindable> Players = new List<IFindable>(5);

        public static int PlayersCount { get; set; }
        public static bool IsGameWon { get; private set; }
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

        public static void GameWon()
        {
            IsGameWon = true;
            OnGameWon?.Invoke();
        }

        public static void Release()
        {
            Players.Clear();
        }
    }
}
