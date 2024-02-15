using Abstraction;
using UnityEngine;

namespace Prefs
{
    public class BTAPlayerPrefs : IGamePlayer
    {
        private const string playerNicknameKey = "player_nickname";
        private const string currentLevelKey = "player_current_lvl";
        private const string currLevelProgressKey = "player_currlvl_progress";

        private string _nickname;

        private int _currentLevel;

        private float _currentLevelProgress;

        public string Nickname => _nickname;

        public int CurrentLevel => _currentLevel;

        public float CurrentLevelProgress => _currentLevelProgress;

        public BTAPlayerPrefs()
        {

        }

        public bool Load()
        {
            var isUserDataExist = CheckDataExist();

            if (isUserDataExist == false)
                return false;

            _nickname = PlayerPrefs.GetString(playerNicknameKey);
            _currentLevel = PlayerPrefs.GetInt(currentLevelKey);
            _currentLevelProgress = PlayerPrefs.GetFloat(currLevelProgressKey);

            return true;
        }

        private bool CheckDataExist()
        {
            if (PlayerPrefs.HasKey(playerNicknameKey) == false)
                return false;
            if (PlayerPrefs.HasKey(currentLevelKey) == false)
                return false;
            if (PlayerPrefs.HasKey(currLevelProgressKey) == false)
                return false;

            return true;
        }

        public void Save(IGamePlayer player)
        {
            _nickname = player.Nickname;
            _currentLevel = player.CurrentLevel;
            _currentLevelProgress = player.CurrentLevelProgress;

            PlayerPrefs.SetString(playerNicknameKey, player.Nickname);
            PlayerPrefs.SetInt(currentLevelKey, player.CurrentLevel);
            PlayerPrefs.SetFloat(currLevelProgressKey, player.CurrentLevelProgress);
        }
    }
}
