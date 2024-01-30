using UnityEngine;
using TMPro;

namespace Tools
{

    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _loadingText;

        [SerializeField]
        private Transform _loaddingProgressPlace;

        public Transform LoaddingProgressPlace => _loaddingProgressPlace;

        public void InitUI(string loadingText)
        {
            _loadingText.text = loadingText;
        }
    }
}
