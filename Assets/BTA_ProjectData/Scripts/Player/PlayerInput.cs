using System;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerInput : MonoBehaviour
    {
        public static event Action OnShootInput;
        public static event Action OnReloadInput;
        public static event Action OnPauseInput;

        public static event Action<string> OnNameChanged;
        public static event Action<float> OnHealthChanged;
        public static event Action<int> OnLevelChanged;
        public static event Action<int> OnAmmoChanged;

        [SerializeField]
        private KeyCode _reloadKey = KeyCode.R;

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                OnShootInput?.Invoke();
            }
            if (Input.GetKeyDown(_reloadKey))
            {
                OnReloadInput?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPauseInput?.Invoke();
            }
        }

        public static void ChangeName(string value) 
            => OnNameChanged?.Invoke(value);

        public static void ChangeHealth(float value) 
            => OnHealthChanged?.Invoke(value);

        public static void ChangeLevel(int value)
           => OnLevelChanged?.Invoke(value);

        public static void ChangeAmmo(int value)
           => OnAmmoChanged?.Invoke(value);

    }
}
