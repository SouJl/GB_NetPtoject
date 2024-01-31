using Abstraction;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools
{
    public class LoadingScreenController : BaseController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/LoadingScreen");

        private readonly LoadingScreenUI _view;

        private ProgressController _connectionProgress;

        public LoadingScreenController(
            Transform placeForUI, LoadingScreenType type)
        {
            _view = LoadView(placeForUI);

            _view.InitUI(GetLoadingText(type));

            _connectionProgress = new ProgressController(_view.LoaddingProgressPlace);
        }

        private LoadingScreenUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LoadingScreenUI>();
        }

        private string GetLoadingText(LoadingScreenType type)
        {
            switch (type)
            {
                default:
                    return string.Empty;

                case LoadingScreenType.GameLoading:
                    {
                        return "CONNECT TO GAME...";
                    }
                case LoadingScreenType.LobbyLoading:
                    {
                        return "JOIN IN LOBBY...";
                    }
            }
        }

        public void Start()
        {
            _connectionProgress.Start();
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
            
            _connectionProgress?.Dispose();
        }
    }
}
