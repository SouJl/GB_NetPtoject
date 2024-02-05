
using Photon.Realtime;

namespace GameLobby
{
    public class BTAPlayer
    {
        private bool _isReady;

        private Player _playerData;

        public bool IsReady 
        { 
            get => _isReady; 
            set => _isReady = value; 
        }
        public Player PlayerData 
        {
            get => _playerData; 
            set => _playerData = value; 
        }

        public BTAPlayer(Player playerdData)
        {
            _playerData = playerdData;
            _isReady = false;
        }
    }
}
