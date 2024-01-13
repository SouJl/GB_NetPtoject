using UnityEngine;
using Photon.Pun;
using UI;
using Photon.Realtime;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomName;
    [SerializeField]
    private PhotonPanelUI _photonPanelUI;
    [SerializeField]
    private DebugConsoleUI _debugConsoleUI;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _photonPanelUI.OnConncect += ConnectToPhotonServer;
        _photonPanelUI.OnDisconncect += DisconnectRomPhotonServer;
    }

    private void ConnectToPhotonServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            _debugConsoleUI.LogWarning("Can't execute connect while still connected to Photon");
            return;
        }
           

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    private void DisconnectRomPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            _debugConsoleUI.LogWarning("Can't execute disconnect while not connected to Photon");
            return;
        }
           

        PhotonNetwork.Disconnect();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        _debugConsoleUI.Log("OnConnectedToMaster");

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
        var joinedRoomMessage = $"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}";

        Debug.Log(joinedRoomMessage);

        _debugConsoleUI.Log(joinedRoomMessage);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconenctedFromMaster");

        _debugConsoleUI.Log("OnDisconenctedFromMaster");
    }
}
