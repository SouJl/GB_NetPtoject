using UnityEngine;

namespace Tools
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private void Update()
        {
            if (!_target)
                return;

            transform.position = _target.position;
        }
    }
}
