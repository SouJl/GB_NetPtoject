using UnityEngine;

namespace Tools
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField]
        private float _sensX = 100f;
        [SerializeField]
        private float _sensY = 100f;

        [SerializeField]
        private Transform _orientation;

        private float _xRotation;
        private float _yRotation;

        private float _deltaTime;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _deltaTime = Time.deltaTime;
        }

        private void Update()
        {
            var mouseX = Input.GetAxisRaw("Mouse X") * _deltaTime * _sensX;
            var mouseY = Input.GetAxisRaw("Mouse Y") * _deltaTime * _sensY;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _yRotation += mouseX;

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
    }
}
