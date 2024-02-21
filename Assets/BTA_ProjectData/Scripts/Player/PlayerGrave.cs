using Photon.Pun;
using PlayFab.EconomyModels;
using System.Collections;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerGrave : MonoBehaviourPun
    {
        [SerializeField]
        private Transform _cameraHolder;
        [SerializeField]
        private float _smooth = 0.5f;

        private Camera _mainCamera;

        private bool _isInitialized = false;

        private PlayerView _view;

        private Vector3 _originalPos;
        private Quaternion _originalRotation;

        public void Init(PlayerView creator, Camera camera)
        {
            if (!photonView.IsMine)
                return;

            _view = creator;

            _mainCamera = camera;

            _originalPos = _mainCamera.transform.position;
            _originalRotation = _mainCamera.transform.rotation;

            _mainCamera.transform.parent = _cameraHolder;

            _isInitialized = true;

            StartCoroutine(WhaitToRevive());
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            _mainCamera.transform.position = Vector3.Slerp(_mainCamera.transform.position, _cameraHolder.position, Time.deltaTime * _smooth);

        }

        private IEnumerator WhaitToRevive()
        {
            yield return new WaitForSeconds(3f);

            _isInitialized = false;
            
            _mainCamera.transform.position = Vector3.zero;

            _view.Revive();

            yield return new WaitForSeconds(0.2f);

            PhotonNetwork.Destroy(gameObject);
        }
    }
}
