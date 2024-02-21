using Abstraction;
using UnityEngine;

namespace Weapon
{
    public class WeaponSway : MonoBehaviour, IPaused
    {
        [Header("Weapon Sway Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float sensitivityMultiplier;



        private void Update()
        {
            if (_isPaused)
                return;

            // get mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityMultiplier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityMultiplier;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
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
