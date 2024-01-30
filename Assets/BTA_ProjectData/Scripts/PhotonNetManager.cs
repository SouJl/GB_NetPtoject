using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Configs;
using UI;
using Abstraction;

public class PhotonNetManager : MonoBehaviourPunCallbacks
{
    private TypedLobby _lobby;

    public event Action OnConnectedToServer;
    public event Action OnDisConnectedFromServer;

    public event Action OnJoinInLobby;
    public event Action OnLeftFromLobby;


    public event Action<Room> OnJoinInRoom;
    public event Action OnLeftFromRoom;
    public event Action<List<RoomInfo>> OnRoomsUpdate;
    public event Action<Player> OnPlayerEnterInRoom;

    public void Init(GameConfig gameConfig)
    {
        _lobby = new TypedLobby(gameConfig.PhotonLobbyName, LobbyType.Default);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {

            Debug.LogWarning("Can't execute connect while still connected to Photon");
            return;
        }


        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }


    public void SetUserData(string nickName)
    {
        PhotonNetwork.LocalPlayer.NickName = nickName;
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(_lobby);
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void CreateRoom(CreationRoomData data)
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = data.MaxPlayers,
        };

        PhotonNetwork.JoinOrCreateRoom(data.RoomName, roomOptions, _lobby);
    }

    public Player[] GetPlayerInRomm()
    {
        return PhotonNetwork.PlayerList;
    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        OnConnectedToServer?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"OnDisconenctedFromMaster by {cause} cause");
        OnDisConnectedFromServer?.Invoke();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        
        OnJoinInLobby?.Invoke();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");

        OnLeftFromLobby?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        if (!PhotonNetwork.InRoom)
            return;

        OnJoinInRoom?.Invoke(PhotonNetwork.CurrentRoom);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        OnRoomsUpdate?.Invoke(roomList);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        OnPlayerEnterInRoom?.Invoke(newPlayer);
    }

    #endregion
}


