﻿using UnityEngine;

namespace Tools
{
    public class FirstPersonCamera : MonoBehaviour
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

        public void Init(Camera camera)
        {
            _camera = camera;

            Cursor.lockState = CursorLockMode.Locked;

            Cursor.visible = false;

            _isFollowing = true;
        }

        private void Update()
        {
            if (_isFollowing == false)
                return;

            if (!_target || !_orientationPoint)
                return;

            var mouseX = Input.GetAxisRaw("Mouse X") * _deltaTime * _sensX;
            var mouseY = Input.GetAxisRaw("Mouse Y") * _deltaTime * _sensY;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _yRotation += mouseX;

            _camera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientationPoint.rotation = Quaternion.Euler(0, _yRotation, 0);

            _camera.transform.position = _target.position;
        }

        private void OnDrawGizmos()
        {
            if (_syncInEditor == false)
                return;

            if (!_camera || !_target || !_orientationPoint)
                return;

            _camera.transform.position = _target.position;
        }

    }
}
