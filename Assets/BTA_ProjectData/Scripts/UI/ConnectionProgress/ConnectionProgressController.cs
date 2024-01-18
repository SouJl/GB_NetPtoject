using Abstraction;
using Tools;
using UnityEngine;

namespace UI
{
    public class ConnectionProgressController : IController, IOnUpdate
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/ConnectionProgress");

        private readonly ConnectionProgressUI _view;
        private readonly float _minValue;
        private readonly float _maxValue;
        private readonly float _speed;

        private float _currentProgress;
        private bool _isEnable;

        public ConnectionProgressController(Transform placeForUI)
        {
            _view = LoadView(placeForUI);
            _view.InitUI();

            _minValue = _view.MinValue;
            _maxValue = _view.MaxValue;
            _speed = _view.Speed;
        }

        private ConnectionProgressUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);
            return objectView.GetComponent<ConnectionProgressUI>();
        }

        public void Start()
        {
            _view.Show();

            _currentProgress = 0;
            _view.UpdateProgressValue(_currentProgress);

            _isEnable = true;
        }

        public void Stop() 
        {
            _currentProgress = 0;
            _view.UpdateProgressValue(_currentProgress);

            _isEnable = false;

            _view.Hide();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if (_isEnable == false)
                return;

            var resultValue = Mathf.Lerp(_minValue, _maxValue, _currentProgress);
            
            _currentProgress += deltaTime * _speed;

            if (_currentProgress >= _maxValue)
                _currentProgress = 0;

            _view.UpdateProgressValue(resultValue);
        }
    }
}
