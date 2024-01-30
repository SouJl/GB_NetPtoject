using Abstraction;
using Tools;
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
            Transform placeForUI)
        {
            _view = LoadView(placeForUI);

            _connectionProgress = new ProgressController(_view.LoaddingProgressPlace);

            _connectionProgress.Start();
        }

        private LoadingScreenUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<LoadingScreenUI>();
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
