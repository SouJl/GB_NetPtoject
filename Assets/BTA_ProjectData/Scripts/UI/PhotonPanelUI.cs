using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PhotonPanelUI : MonoBehaviour
    {
        [SerializeField]
        private Button _conenctButton;
        [SerializeField]
        private Button _disconenctButton;

        public Action OnConncect;
        public Action OnDisconncect;

        private void Start()
        {
            _conenctButton.onClick.AddListener(() => OnConncect?.Invoke());
            _disconenctButton.onClick.AddListener(() => OnDisconncect?.Invoke());
        }

        private void OnDestroy()
        {
            _conenctButton.onClick.RemoveAllListeners();
            _disconenctButton.onClick.RemoveAllListeners();
        }
    }
}
