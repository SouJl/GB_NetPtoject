using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Configs;
using Abstraction;
using ExitGames.Client.Photon;
using UnityEditor;

namespace MultiplayerService
{
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

        public Player CurrentPlayer => PhotonNetwork.LocalPlayer;

        public bool IsRoomOpen
        {
            get => PhotonNetwork.CurrentRoom.IsOpen;
            set
            {
                PhotonNetwork.CurrentRoom.IsOpen = value;
            }
        }

        public bool IsRoomVisiable
        {
            get => PhotonNetwork.CurrentRoom.IsVisible;
            set
            {
                PhotonNetwork.CurrentRoom.IsVisible = value;
            }
        }


        public void Init(GameConfig gameConfig)
        {
            _lobby = new TypedLobby(gameConfig.PhotonLobbyName, LobbyType.Default);

            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void Connect(string userId)
        {
            if (PhotonNetwork.IsConnected)
            {

                Debug.LogWarning("Can't execute connect while still connected to Photon");
                return;
            }

            PhotonNetwork.AuthValues = new AuthenticationValues
            {
                UserId = userId
            };

            PhotonNetwork.GameVersion = Application.version;
            PhotonNetwork.EnableCloseConnection = true;

            PhotonNetwork.ConnectUsingSettings();
        }

        public void Disconnect()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.Disconnect();
        }

        public void SetUserData(UserData data)
        {
            PhotonNetwork.LocalPlayer.NickName = data.UserName;

            var authValue = new AuthenticationValues
            {
                UserId = GUID.Generate().ToString()
            };

            PhotonNetwork.AuthValues = authValue;
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
                IsVisible = data.IsPublic,
                PublishUserId = data.PublishUserId
            };

            PhotonNetwork.JoinOrCreateRoom(data.RoomName, roomOptions, _lobby, data.Whitelist);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public Player[] GetPlayerInRoom()
        {
            return PhotonNetwork.PlayerList;
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

        public void FindFriends(string[] players)
        {
            PhotonNetwork.FindFriends(players);
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

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"OnJoinRoomFailed: [{returnCode}] {message}");
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

       /* public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Debug.Log("OnFriendListUpdate");
            for (int i = 0; i < friendList.Count; i++)
            {
                if (friendList[i].IsOnline == false)
                    continue;
                Debug.Log($"PlayerId: {friendList[i].UserId}");
                if (friendList[i].IsInRoom)
                {
                    Debug.Log($"PlayerRoom: {friendList[i].Room}");
                }
            }
        }*/

        #endregion
    }
}



