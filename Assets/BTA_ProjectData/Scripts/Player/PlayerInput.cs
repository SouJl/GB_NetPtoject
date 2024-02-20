﻿using System;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerInput : MonoBehaviour
    {
        public static event Action OnShootInput;
        public static event Action OnReloadInput;

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
        }
    }
}