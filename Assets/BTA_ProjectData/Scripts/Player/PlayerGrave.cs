using Photon.Pun;
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

        public void Init(Camera camera)
        {
            if (!photonView.IsMine)
                return;

            _mainCamera = camera;

            _mainCamera.transform.parent = _cameraHolder;

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            _mainCamera.transform.position = Vector3.Slerp(_mainCamera.transform.position, _cameraHolder.position, Time.deltaTime * _smooth);

            _mainCamera.transform.rotation = Quaternion.Slerp(_mainCamera.transform.rotation, _cameraHolder.rotation, Time.deltaTime * _smooth);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
                return;

            if(other.gameObject.tag == "Player")
            {
                Debug.Log(other.gameObject);
            }

        }
    }
}
