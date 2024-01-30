using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abstraction
{
    public class BaseController : IController, IDisposable
    {
        private List<GameObject> _gameObjects;
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            DisposeGameObjects();

            OnDispose();
        }

        private void DisposeGameObjects()
        {
            if (_gameObjects == null)
                return;

            for (int i = 0; i < _gameObjects.Count; i++) 
            {
                var gameObject = _gameObjects[i];
                UnityEngine.Object.Destroy(gameObject);
            } 
               
            _gameObjects.Clear();
        }

        protected virtual void OnDispose() { }

        protected void AddGameObject(GameObject gameObject)
        {
            _gameObjects ??= new List<GameObject>();
            _gameObjects.Add(gameObject);
        }
    }
}
