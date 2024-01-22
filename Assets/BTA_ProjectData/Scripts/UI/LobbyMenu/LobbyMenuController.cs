using Abstraction;
using MultiplayerService;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class LobbyMenuController : BaseUIController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LobbyMenu");

        private readonly LobbyMenuView _view;
        private readonly GamePrefs _gamePrefs;
        private readonly IMultiplayerService _multiplayerService;

        private ConnectionProgressController _connectionProgress;

        public LobbyMenuController(
            Transform placeForUI, 
            GamePrefs gamePrefs, 
            IMultiplayerService multiplayerService)
        {
            _view = LoadView(placeForUI);
            _view.InitUI();

            _gamePrefs = gamePrefs;
            _multiplayerService = multiplayerService;

            _connectionProgress = new ConnectionProgressController(_view.ConnetcionProgressPlacement);

            Subscribe();

            LoadUserData(_gamePrefs.UserId);
        }

        private void Subscribe()
        {
            _multiplayerService.OnGetAccountSuccess += GetAccountSuccessed;
            _multiplayerService.OnGetAccountFailure += GetAccountFailed;

            _multiplayerService.OnGetCatalogItemsSuccess += GetCatalogItems;
        }

        private void Unsubscribe()
        {
            _multiplayerService.OnGetAccountSuccess += GetAccountSuccessed;
            _multiplayerService.OnGetAccountFailure += GetAccountFailed;
            _multiplayerService.OnGetCatalogItemsSuccess -= GetCatalogItems;
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

            _connectionProgress.Start();
        }

        private void GetAccountSuccessed(UserData userData)
        {
            _connectionProgress.Stop();

            _view.ShowUserData();

            _multiplayerService.GetAvilableUserItems(userData.Id);

            UpdateUserDataUI(userData);
        }

        private void UpdateUserDataUI(UserData userData)
        {
            _view.SetUserName(userData.UserName);
            _view.SetUserCreatedTime(userData.CreatedTime.ToString());
        }

        private void GetAccountFailed(string errorMessage)
        {
            Debug.LogError($"Something went wrong: {errorMessage}");

            _connectionProgress.Stop();

            _gamePrefs.ChangeGameState(Enumerators.GameState.Authentication);
        }


        private void GetCatalogItems(IList<CatalogItem> items)
        {
            for(int i =0; i < items.Count; i++)
            {
                Debug.Log($"ItemId: {items[i].ItemId}");
            }
        }


        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _connectionProgress.ExecuteUpdate(deltaTime);
        }
    }
}
