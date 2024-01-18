using Abstraction;
using Configs;
using PlayFab;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public class AuthenticationMenuController : IController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/AuthenticationMenu");

        private AuthenticationMenuView _view;

        public AuthenticationMenuController(Transform placeForUi, GameConfig gameConfig)
        {
            _view = LoadView(placeForUi);
            _view.InitView();

            InitializeServie(gameConfig._PlayFabTitleId);
        }

        private AuthenticationMenuView LoadView(Transform placeForUi)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUi, false);
            return objectView.GetComponent<AuthenticationMenuView>();
        }

        private void InitializeServie(string titleId)
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = titleId;
            }
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _view.ExecuteUpdate(deltaTime);
        }
    }
}
