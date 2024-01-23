﻿using Abstraction;
using Codice.CM.Common;
using Configs;
using MultiplayerService;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace UI
{
    public class ItemsContainerController : BaseUIController, IOnUpdate
    {
        private readonly ItemsInfoConfig _itemsInfoConfig;
        private readonly ItemsContainerUI _containerView;
        private readonly IMultiplayerService _multiplayerService;

        private ConnectionProgressController _loadCatalogItemsProgress;

        private List<ItemModel> _availableCatalogItems;

        public ItemsContainerController(
            ItemsInfoConfig itemsInfoConfig,
            ItemsContainerUI containerView,
            Transform loadCatalogItemsProgressPos,
            IMultiplayerService multiplayerService)
        {
            _itemsInfoConfig = itemsInfoConfig;
            _containerView = containerView;
            _multiplayerService = multiplayerService;

            _loadCatalogItemsProgress
                = new ConnectionProgressController(loadCatalogItemsProgressPos);

            Subscribe();

            _containerView.Hide();

            LodaCatalogItems(_itemsInfoConfig.CatalogVersion);
        }

        private void Subscribe()
        {
            _multiplayerService.OnGetCatalogItemsSuccess += GetCatalogItems;
            _multiplayerService.OnError += GetDataFailed;
        }

        private void Unsubscribe()
        {
            _multiplayerService.OnGetCatalogItemsSuccess -= GetCatalogItems;
            _multiplayerService.OnError -= GetDataFailed;
        }

        private void GetCatalogItems(List<CatalogItem> resultItems)
        {
            _availableCatalogItems = GetAvailableItems(resultItems);
           
            InitItemsContainer(_availableCatalogItems);

            _loadCatalogItemsProgress.Stop();
        }

        private List<ItemModel> GetAvailableItems(List<CatalogItem> resultItems)
        {
            var result = new List<ItemModel>();

            var items = _itemsInfoConfig.AvailableItemsInfo;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                var availableCatalogItem = resultItems.Find(itm => itm.ItemId == item.ItemId);

                if (availableCatalogItem != null)
                {
                    result.Add(new ItemModel
                    {
                        Id = availableCatalogItem.ItemId,
                        Name = availableCatalogItem.DisplayName,
                        Type = item.Type,
                        Icon = item.ItemIcon
                    });
                }
            }

            return result;
        }

        private void InitItemsContainer(List<ItemModel> availableItems)
        {
            _containerView.Display(availableItems);
            _containerView.Show();
        }

        private void GetDataFailed(string errorMessage)
        {
            Debug.LogError($"Something went wrong: {errorMessage}");

            _loadCatalogItemsProgress.Stop();
        }

        private void LodaCatalogItems(string catalogVersion)
        {
            _multiplayerService.GetCatalogItems(catalogVersion);

            _loadCatalogItemsProgress.Start();
        }


        public void ExecuteUpdate(float deltaTime)
        {
            _loadCatalogItemsProgress.ExecuteUpdate(deltaTime);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _containerView.Clear();

            Unsubscribe();
        }
    }
}
