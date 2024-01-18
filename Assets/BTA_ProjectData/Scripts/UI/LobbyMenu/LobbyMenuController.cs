using Abstraction;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class LobbyMenuController : BaseUIController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LobbyMenu");

        private readonly LobbyMenuView _view;
        private readonly GamePrefs _gamePrefs;

        public LobbyMenuController(Transform placeForUI, GamePrefs gamePrefs)
        {
            _view = LoadView(placeForUI);

            _gamePrefs = gamePrefs;
        }

        private LobbyMenuView LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LobbyMenuView>();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}
