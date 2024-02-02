using Photon.Realtime;
using System;

namespace GameLobby
{
    public interface IRoomMenuUI
    {
        public event Action OnStartGamePressed;
        public event Action OnExitPressed;

        public void InitUI(string roomName);

        public void AddPlayer(Player player);

        public void RemovePlayer(Player player);

        public void UpdatePlayerData(Player target, string state);
    }
}
