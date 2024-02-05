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
        private readonly StateTransition _stateTransition;

        private readonly LoadingScreenUI _view;

        private ProgressController _connectionProgress;

        public LoadingScreenController(
            Transform placeForUI, 
            IGamePrefs gamePrefs, 
            GameNetManager netManager,
            StateTransition stateTransition)
        {
            _gamePrefs = gamePrefs;
            _netManager = netManager;
            _stateTransition = stateTransition;

            _view = LoadView(placeForUI);

            _view.InitUI("CONNECT TO GAME...");

            _connectionProgress = new ProgressController(_view.LoaddingProgressPlace);
           
            _netManager.OnConnectedToServer += Connected;
        }

        private LoadingScreenUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LoadingScreenUI>();
        }

        private void Connected()
        {
            _stateTransition.Invoke(
                () => _gamePrefs.ChangeGameState(GameState.MainMenu));
        }
        public void Start()
        {
            _connectionProgress.Start();

            _netManager.Connect(_gamePrefs.Data.UserName);
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

            _netManager.OnConnectedToServer -= Connected;
            
            _connectionProgress?.Dispose();
        }
    }
}
