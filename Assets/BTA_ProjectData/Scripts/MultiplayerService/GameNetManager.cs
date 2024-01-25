using Abstraction;
using Configs;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerService
{
    public class GameNetManager : IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IOnUpdate, IDisposable
    {
        private readonly LoadBalancingClient _loadBalancingClient;

        private ClientState _netState;
        
        public ClientState NetState => _netState;

        public event Action<ClientState> OnNetStateChanged;
        
        public GameNetManager(GameConfig gameConfig)
        {
            _loadBalancingClient = new LoadBalancingClient();
            
            _loadBalancingClient.StateChanged += NetStateChanged;

            _loadBalancingClient.AddCallbackTarget(this);

            _loadBalancingClient.ConnectUsingSettings(gameConfig.PhotonServerSettings.AppSettings);
        }

        private void NetStateChanged(ClientState prevState, ClientState currentState)
        {
            _netState = currentState;

            OnNetStateChanged?.Invoke(_netState);
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if (_loadBalancingClient == null)
                return;

            _loadBalancingClient.Service();
        }


        #region IConnectionCallbacks

        public void OnConnected()
        {
            Debug.Log("GameNetManager_" + "OnConnected");
        }

        public void OnConnectedToMaster()
        {
            Debug.Log("GameNetManager_" + "OnConnectedToMaster");
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log("GameNetManager_" + "OnCustomAuthenticationFailed");
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            Debug.Log("GameNetManager_" + "OnCustomAuthenticationResponse");
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("GameNetManager_" + "OnDisconnected");
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            Debug.Log("GameNetManager_" + "OnRegionListReceived");
        }

        #endregion

        #region IMatchmakingCallbacks

        public void OnCreatedRoom()
        {
            Debug.Log("GameNetManager_" + "OnCreatedRoom");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("GameNetManager_" + "OnCreateRoomFailed");
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            Debug.Log("GameNetManager_" + "OnFriendListUpdate");
        }

        public void OnJoinedRoom()
        {
            Debug.Log("GameNetManager_" + "OnJoinedRoom");
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("GameNetManager_" + "OnJoinRandomFailed");
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("GameNetManager_" + "OnJoinRoomFailed");
        }

        public void OnLeftRoom()
        {
            Debug.Log("GameNetManager_" + "OnLeftRoom");
        }

        #endregion

        #region ILobbyCallbacks

        public void OnJoinedLobby()
        {
            Debug.Log("GameNetManager_" + "OnJoinedLobby");
        }

        public void OnLeftLobby()
        {
            Debug.Log("GameNetManager_" + "OnLeftLobby");
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.Log("GameNetManager_" + "OnLobbyStatisticsUpdate");
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("GameNetManager_" + "OnRoomListUpdate");
        }

        #endregion

        #region IDisposable

        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _loadBalancingClient.StateChanged -= NetStateChanged;
            
            _loadBalancingClient.RemoveCallbackTarget(this);
            
            _isDisposed = true;
        }

        #endregion
    }
}
