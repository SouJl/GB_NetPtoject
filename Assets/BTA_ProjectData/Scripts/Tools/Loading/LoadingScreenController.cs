using Abstraction;
using Enumerators;
using MultiplayerService;
using Prefs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools
{
    public class LoadingScreenController : BaseController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LoadingScreen");

        private readonly IGamePrefs _gamePrefs;
        private readonly GameNetManager _netManager;
        private readonly DataServerService _serverService;
        private readonly StateTransition _stateTransition;

        private readonly LoadingScreenUI _view;

        private ProgressController _connectionProgress;

        public LoadingScreenController(
            Transform placeForUI, 
            IGamePrefs gamePrefs, 
            GameNetManager netManager,
            DataServerService serverService,
            StateTransition stateTransition)
        {
            _gamePrefs = gamePrefs;
            _netManager = netManager;
            _serverService = serverService;

            _stateTransition = stateTransition;

            _view = LoadView(placeForUI);

            _view.InitUI("CONNECT TO GAME...");

            _connectionProgress = new ProgressController(_view.LoaddingProgressPlace);

            Subscribe();
        }

        private LoadingScreenUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LoadingScreenUI>();
        }

        private void Subscribe()
        {
            _serverService.OnLogInSucceed += LoginnedInGame;
            _serverService.OnGetUserData += LoadedUserDataFromServer;
            _netManager.OnConnectedToServer += Connected;
        }

        private void Unsubscribe()
        {
            _serverService.OnLogInSucceed -= LoginnedInGame;
            _serverService.OnGetUserData -= LoadedUserDataFromServer;
            _netManager.OnConnectedToServer -= Connected;
        }

        private void LoginnedInGame(string userId)
        {
            _serverService.GetPlayerData(userId);
        }

        private void LoadedUserDataFromServer(PlayfabPlayerData player)
        {
            Debug.Log($"Getted User : {player.Nickname} Lvl[{player.CurrentLevel}] with progress {player.CurrentLevelProgress}");

            _gamePrefs.SetPlayer(player);

            _netManager.Connect(player.Nickname);
        }

        private void Connected()
        {
            _stateTransition.Invoke(
                () => _gamePrefs.ChangeGameState(GameState.MainMenu));
        }

        public void Start()
        {
            _connectionProgress.Start();

            _serverService.LogIn(_gamePrefs.GetUser());
        }

        public void Stop()
        {
            _connectionProgress.Stop();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _connectionProgress.ExecuteUpdate(deltaTime);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
            
            _connectionProgress?.Dispose();
        }
    }
}
