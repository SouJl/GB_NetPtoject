using Abstraction;
using Configs;
using MultiplayerService;
using Prefs;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class LobbyMenuController : BaseController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LobbyMenu");

        private readonly LobbyMenuView _view;
        private readonly GameConfig _gameConfig;
        private readonly GamePrefs _gamePrefs;
        private readonly IMultiplayerService _multiplayerService;

        private readonly GameNetManager _netManager;

        private ProgressController _loadUserInfoProgress;

        private ItemsContainerController _itemsContainerController;

        public LobbyMenuController(
            Transform placeForUI, 
            GameConfig gameConfig,
            GamePrefs gamePrefs, 
            IMultiplayerService multiplayerService)
        {
            _view = LoadView(placeForUI);
            _view.InitUI();

            _gameConfig = gameConfig;
            
            _gamePrefs = gamePrefs;
            _multiplayerService = multiplayerService;

            _netManager = new GameNetManager(gameConfig);

            _loadUserInfoProgress 
                = new ProgressController(_view.LoadUserInfoPlacement);

            _itemsContainerController 
                = new ItemsContainerController(
                    _gameConfig.ItemsInfoConfig, 
                    _view.ItemsContainer, 
                    _view.LoadCatalogItemsProgressPlacement, 
                    multiplayerService);

            Subscribe();

            LoadUserData(_gamePrefs.UserId);
        }

        private void Subscribe()
        {
            _multiplayerService.OnGetAccountSuccess += GetAccountSuccessed;

            _multiplayerService.OnError += GetDataFailed;
        }

        private void Unsubscribe()
        {
            _multiplayerService.OnGetAccountSuccess -= GetAccountSuccessed;
            _multiplayerService.OnError -= GetDataFailed;
        }

        private LobbyMenuView LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LobbyMenuView>();
        }

        private void LoadUserData(string userId)
        {
            _multiplayerService.GetAccountInfo(userId);

            _loadUserInfoProgress.Start();
        }

        private void GetAccountSuccessed(UserData userData)
        {
            _loadUserInfoProgress.Stop();

            _view.ShowUserData();

            UpdateUserDataUI(userData);

            //_netManager.Connect();
        }

        private void UpdateUserDataUI(UserData userData)
        {
            _view.SetUserName(userData.UserName);
            _view.SetUserCreatedTime(userData.CreatedTime.ToString());
        }

        private void GetDataFailed(string errorMessage)
        {
            Debug.LogError($"Something went wrong: {errorMessage}");

            _loadUserInfoProgress.Stop();

            _gamePrefs.ChangeGameState(Enumerators.GameState.Authentication);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _itemsContainerController?.Dispose();

            Unsubscribe();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _netManager.ExecuteUpdate(deltaTime);

            _loadUserInfoProgress.ExecuteUpdate(deltaTime);

            _itemsContainerController.ExecuteUpdate(deltaTime);
        }
    }
}
