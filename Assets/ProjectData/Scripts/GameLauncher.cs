using UnityEngine;
using Photon.Pun;
using UI;
using System;
using Photon.Realtime;

public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomName;
    [SerializeField]
    private PhotonPanelUI _photonPanelUI;

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
            return;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    private void DisconnectRomPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.Disconnect();
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconenctedFromMaster");
    }
}
