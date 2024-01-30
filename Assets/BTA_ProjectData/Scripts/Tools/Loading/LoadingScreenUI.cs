using UnityEngine;

namespace Tools
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _loaddingProgressPlace;

        public Transform LoaddingProgressPlace => _loaddingProgressPlace;
    }
}
