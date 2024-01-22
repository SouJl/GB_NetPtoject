using Abstraction;
using Configs;
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
        private readonly GameConfig _gameConfig;
        private readonly GamePrefs _gamePrefs;
        private readonly IMultiplayerService _multiplayerService;

        private ConnectionProgressController _loadUserInfoProgress;
        private ConnectionProgressController _loadCatalogItemsProgress;

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

            _loadUserInfoProgress 
                = new ConnectionProgressController(_view.LoadUserInfoPlacement);
            _loadCatalogItemsProgress 
                = new ConnectionProgressController(_view.LoadCatalogItemsProgressPlacement);
            Subscribe();

            LoadUserData(_gamePrefs.UserId);
            LoadAvailableItems();
        }

        private void Subscribe()
        {
            _multiplayerService.OnGetAccountSuccess += GetAccountSuccessed;
            _multiplayerService.OnGetCatalogItemsSuccess += GetCatalogItems;

            _multiplayerService.OnError += GetDataFailed;
        }

        private void Unsubscribe()
        {
            _multiplayerService.OnGetAccountSuccess -= GetAccountSuccessed;
            _multiplayerService.OnGetCatalogItemsSuccess -= GetCatalogItems;

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

        private void LoadAvailableItems()
        {
            _multiplayerService.GetCatalogItems();

            _loadCatalogItemsProgress.Start();
        }


        private void GetAccountSuccessed(UserData userData)
        {
            _loadUserInfoProgress.Stop();

            _view.ShowUserData();

            UpdateUserDataUI(userData);
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
            _loadCatalogItemsProgress.Stop();

            _gamePrefs.ChangeGameState(Enumerators.GameState.Authentication);
        }


        private void GetCatalogItems(IList<CatalogItem> items)
        {

            for(int i =0; i < items.Count; i++)
            {
                var itemInfo = GetItemInfo(items[i].ItemId);
                
                if (itemInfo == null)
                    continue;

                _view.ItemsContainer.AddItem(
                    itemInfo.ItemId, 
                    items[i].DisplayName, 
                    itemInfo.ItemIcon);
            }

            _view.ItemsContainer.Show();

            _loadCatalogItemsProgress.Stop();
        }

        private ItemInfo GetItemInfo(string itemId)
        {
            var itemInfoConfigs = _gameConfig.ItemsInfoConfig;

            for(int i =0; i< itemInfoConfigs.WeaponItemsInfoCollection.Count; i++)
            {
                var item = itemInfoConfigs.WeaponItemsInfoCollection[i];
                if(item.ItemId.Equals(itemId))
                {
                    return item;
                }
            }
            for (int i = 0; i < itemInfoConfigs.AmmoItemsInfoCollection.Count; i++)
            {
                var item = itemInfoConfigs.AmmoItemsInfoCollection[i];
                if (item.ItemId.Equals(itemId))
                {
                    return item;
                }
            }
            for (int i = 0; i < itemInfoConfigs.ShieldItemsInfoCollection.Count; i++)
            {
                var item = itemInfoConfigs.ShieldItemsInfoCollection[i];
                if (item.ItemId.Equals(itemId))
                {
                    return item;
                }
            }
            for (int i = 0; i < itemInfoConfigs.ConsumableItemsInfoCollection.Count; i++)
            {
                var item = itemInfoConfigs.ConsumableItemsInfoCollection[i];
                if (item.ItemId.Equals(itemId))
                {
                    return item;
                }
            }

            return null;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _loadUserInfoProgress.ExecuteUpdate(deltaTime);
            _loadCatalogItemsProgress.ExecuteUpdate(deltaTime);
        }
    }
}
