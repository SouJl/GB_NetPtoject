using Abstraction;
using UnityEngine;

namespace Prefs
{

    public class UserPrefs : IGameUser
    {
        private const string _authUserName = "authorization_user_name";
        private const string _authUserPassword = "authorization_user_passw";

        private string _name;
        private string _password;

        public string Name => _name;
        public string Password => _password;

        public UserPrefs()
        {

        }

        public bool Load()
        {
            var isUserDataExist = CheckDataExist();

            if (isUserDataExist == false)
                return false;

            _name = PlayerPrefs.GetString(_authUserName);
            _password = PlayerPrefs.GetString(_authUserPassword);

            return true;
        }

        private bool CheckDataExist()
        {
            if (PlayerPrefs.HasKey(_authUserName) == false)
                return false;
            if (PlayerPrefs.HasKey(_authUserPassword) == false)
                return false;

            return true;
        }


        public void Save(IGameUser user)
        {
            _name = user.Name;
            _password = user.Password;

            PlayerPrefs.SetString(_authUserName, user.Name);
            PlayerPrefs.SetString(_authUserPassword, user.Password);
        }
    }
}
