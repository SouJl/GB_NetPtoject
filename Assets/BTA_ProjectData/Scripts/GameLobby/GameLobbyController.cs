﻿using Abstraction;
using Configs;
using Enumerators;
using MultiplayerService;
using Prefs;
using Tools;
using UnityEngine;

namespace GameLobby
{
    public class GameLobbyController : BaseController, IOnUpdate
    {
        private readonly Transform _placeForUi;
        private readonly GameConfig _gameConfig;
        private readonly IGamePrefs _gamePrefs;
        private readonly GameNetManager _netManager;
        private readonly StateTransition _stateTransition;
        private readonly GameLobbyPrefs _lobbyPrefs;
        
        private readonly LifeCycleController _lobbyLifeCycle;

        private LobbyBrowseController _lobbyBrowseController;
        private CreateGameController _createRoomController;
        private InRoomController _inRoomController;

        public GameLobbyController(
            Transform placeForUI,
            GameConfig gameConfig,
            IGamePrefs gamePrefs,
            GameNetManager netManager,
            StateTransition stateTransition)
        {
            _placeForUi = placeForUI;
            _gameConfig = gameConfig;
            _gamePrefs = gamePrefs;
            _netManager = netManager;
            _stateTransition = stateTransition;

            var user = gamePrefs.GetUser();

            _lobbyPrefs = new GameLobbyPrefs(user.Name);

            _lobbyLifeCycle = new LifeCycleController();

            Subscribe();

            if (gamePrefs.IsSettedGameName)
            {
                _lobbyPrefs.SetRoomData(new CreationRoomData
                {
                    RoomName = gamePrefs.SettedGamName
                });

                _lobbyPrefs.ChangeState(GameLobbyState.InRoom);
            }
            else
            {
                _lobbyPrefs.ChangeState(GameLobbyState.Browse);
            }
        }

        private void Subscribe()
        {
            _lobbyPrefs.OnStateChange += LobbyStateChanged;
            _netManager.OnJoinedInRoomFailed += FailedToJoinInRoom;
        }
        private void Unsubscribe()
        {
            _lobbyPrefs.OnStateChange -= LobbyStateChanged;
            _netManager.OnJoinedInRoomFailed -= FailedToJoinInRoom;
        }

        private void LobbyStateChanged(GameLobbyState state)
        {
            DisposeControllers();

            switch (state)
            {
                default:
                    break;
                case GameLobbyState.Browse:
                    {
                        _lobbyBrowseController
                            = new LobbyBrowseController(_placeForUi, _gameConfig, _lobbyPrefs, _netManager, _stateTransition);

                        _lobbyLifeCycle.AddController(_lobbyBrowseController);
                        break;
                    }
                case GameLobbyState.CreateRoom:
                    {
                        _createRoomController 
                            = new CreateGameController(_placeForUi, _gameConfig, _lobbyPrefs, _netManager, _stateTransition);

                        _lobbyLifeCycle.AddController(_createRoomController);
                        break;
                    }
                case GameLobbyState.InRoom:
                    {
                        _inRoomController 
                            = new InRoomController(_placeForUi, _gameConfig, _lobbyPrefs, _netManager);

                        _lobbyLifeCycle.AddController(_inRoomController);
                        break;
                    }
                case GameLobbyState.StartGame:
                    {
                        StartGame();
                        break;
                    }
                case GameLobbyState.Exit:
                    {
                        ExitFromLobby();
                        break;
                    }
            }
        }

   
        private void DisposeControllers()
        {
            _lobbyLifeCycle?.Dispose();
        }
   
        private void StartGame()
        {
            _gamePrefs.ChangeGameState(GameState.Game);
        }

        private void ExitFromLobby()
        {
            _netManager.OnLeftFromLobby += LeftedFromLobby;
            
            _netManager.LeaveLobby();
        }

        private void LeftedFromLobby()
        {
            _netManager.OnLeftFromLobby -= LeftedFromLobby;

            _gamePrefs.ChangeGameState(GameState.MainMenu);
        }


        private void FailedToJoinInRoom()
        {
            _lobbyPrefs.ChangeState(GameLobbyState.Browse);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            DisposeControllers();

            Unsubscribe();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _lobbyLifeCycle.OnUpdate(deltaTime);
        }
    }
}
