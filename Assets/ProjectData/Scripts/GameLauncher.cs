using UnityEngine;
using Photon.Pun;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomName;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        JoinRoom();
    }

    private void JoinRoom()
    {
        if (PhotonNetwork.InRoom)
            return;
        PhotonNetwork.CreateRoom(_roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }
}
