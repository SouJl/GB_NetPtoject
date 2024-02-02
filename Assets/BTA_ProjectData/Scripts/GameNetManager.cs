using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Configs;
using Abstraction;
using ExitGames.Client.Photon;

public class GameNetManager : MonoBehaviourPunCallbacks
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
    public event Action<Player> OnPlayerLeftFromRoom;
    public event Action<Player, Hashtable> OnPlayerPropsUpdated;

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
        PhotonNetwork.EnableCloseConnection = true;
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
            IsOpen = !data.IClosed,
        };

        PhotonNetwork.JoinOrCreateRoom(data.RoomName, roomOptions, _lobby, data.ReserveSlots);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public Player[] GetPlayerInRoom()
    {
        return PhotonNetwork.PlayerList;
    }

    public void ChangeRoomOpenState(bool state)
    {
        PhotonNetwork.CurrentRoom.IsOpen = state;
    }
    
    public void CloseConnectionToClient(Player client)
    {
        if (client.IsMasterClient)
            return;
        PhotonNetwork.CloseConnection(client);
    }

    public void SetPlayerProperties(Hashtable props)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate");
        OnPlayerPropsUpdated?.Invoke(targetPlayer, changedProps);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        OnPlayerEnterInRoom?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        OnPlayerLeftFromRoom?.Invoke(otherPlayer);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        OnLeftFromRoom?.Invoke();
    }

    #endregion
}


