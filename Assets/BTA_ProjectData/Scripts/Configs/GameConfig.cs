using Photon.Pun;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "B.T.A/" + nameof(GameConfig))]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public string PlayFabTitleId { get; private set; }
        [field: SerializeField] public string PhotonLobbyName { get; private set; }
        [field: SerializeField] public int RoomMinPlayers { get; private set; } = 0;
        [field: SerializeField] public int RoomMaxPlayers { get; private set; } = 10;
        [field: SerializeField] public ItemsInfoConfig ItemsInfoConfig { get; private set; }
        [field: SerializeField] public ServerSettings PhotonServerSettings { get; private set; }
    }
}
