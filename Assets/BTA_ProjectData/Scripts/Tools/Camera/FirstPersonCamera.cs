using Abstraction;
using UnityEngine;

namespace Tools
{
    public class FirstPersonCamera : MonoBehaviour, IPaused
    {
        [SerializeField]
        private float _sensX = 100f;
        [SerializeField]
        private float _sensY = 100f;

        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform _orientationPoint;
        [SerializeField]
        private Transform _playerObj;

        [SerializeField]
        private bool _syncInEditor = false;
        [SerializeField]
        private Camera _camera;

        private float _xRotation;
        private float _yRotation;

        private float _deltaTime;


        private bool _isFollowing;

        private void Awake()
        {
            _deltaTime = Time.deltaTime;
            _isFollowing = false;


        }

        public void Init()
        {

            _isFollowing = true;
        }

        public void SetCamera(Camera camera) 
        {
            _camera = camera;

            _camera.transform.parent = transform;
        }

        private void Update()
        {
            if (GameStateManager.IsGameWon)
                return;

            if (_isPaused)
                return;

            if (_isFollowing == false)
                return;

            if (!_target || !_orientationPoint)
                return;

            var mouseX = Input.GetAxisRaw("Mouse X") * _deltaTime * _sensX;
            var mouseY = Input.GetAxisRaw("Mouse Y") * _deltaTime * _sensY;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _yRotation += mouseX;

            transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0);

            _orientationPoint.rotation = Quaternion.Euler(0, _yRotation, 0);

            _camera.transform.position = _target.position;
        }

        private void OnDrawGizmos()
        {
            if (_isPaused)
                return;

            if (_syncInEditor == false)
                return;

            if (!_camera || !_target || !_orientationPoint)
                return;

            _camera.transform.position = _target.position;
        }


        #region IPaused

        private bool _isPaused;

        public void OnPause(bool state)
        {
            _isPaused = state;
        }

        #endregion

    }
}
