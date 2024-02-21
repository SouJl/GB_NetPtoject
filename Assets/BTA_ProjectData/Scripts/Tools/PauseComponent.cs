using Abstraction;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class PauseComponent : MonoBehaviour
    {
        private List<IPaused> _pausedObjects;

        private void Awake()
        {
            var pauseComponents = gameObject.GetComponentsInChildren<IPaused>();

            _pausedObjects = new();

            for(int i =0; i < pauseComponents.Length; i++)
            {
                _pausedObjects.Add(pauseComponents[i]);
            }

            GameStateManager.OnGamePaused += OnPause;
        }

        private void OnPause(bool state)
        {
            for (int i = 0; i < _pausedObjects.Count; i++)
            {
                _pausedObjects[i].OnPause(state);
            }
        }

        private void OnDestroy()
        {
            GameStateManager.OnGamePaused -= OnPause;
        }
    }
}
