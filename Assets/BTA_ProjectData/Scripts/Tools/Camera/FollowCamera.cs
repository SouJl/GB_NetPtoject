using Abstraction;
using UnityEngine;

namespace Tools
{
    public class FollowCamera : MonoBehaviour, IPaused
    {
        [SerializeField]
        private Transform _target;

        private void Update()
        {
            if (_isPaused)
                return;

            if (!_target)
                return;

            transform.position = _target.position;
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
