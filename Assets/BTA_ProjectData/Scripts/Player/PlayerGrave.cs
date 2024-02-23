﻿using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerGrave : MonoBehaviourPun
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Transform _camerStartPoint;
        [SerializeField]
        private Transform _camerEndPoint;
        [SerializeField]
        private float _smooth = 0.5f;
        [SerializeField]
        private float _deadTime = 3f;

        private bool _isInitialized = false;

        private PlayerView _view;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                _camera.gameObject.SetActive(true);
            }
            else
            {
                _camera.gameObject.SetActive(false);
            }
        }

        public void Init(PlayerView creator)
        {
            if (!photonView.IsMine)
                return;

            _view = creator;
            
            _camera.transform.position = _camerStartPoint.position;
            _camera.transform.rotation = _camerStartPoint.rotation;

            _isInitialized = true;

            StartCoroutine(WhaitToRevive());
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            _camera.transform.position
                       = Vector3.Slerp(_camera.transform.position, _camerEndPoint.position, Time.deltaTime * _smooth);
            _camera.transform.rotation
                   = Quaternion.Slerp(_camera.transform.rotation, _camerEndPoint.rotation, Time.deltaTime * _smooth);

        }

        private IEnumerator WhaitToRevive()
        {
            yield return new WaitForSeconds(_deadTime);

            _isInitialized = false;

            _view.Revive();
                     
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
