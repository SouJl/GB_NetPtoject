using Abstraction;
using Enumerators;
using Photon.Pun;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using Weapon;

namespace BTAPlayer
{
    public class PlayerView : MonoBehaviourPunCallbacks, IPunObservable, IDamageable, IFindable
    {
        [SerializeField]
        private Transform _selfTransform;
        [SerializeField]
        private Transform _orientation;
        [SerializeField]
        private Transform _playerObj;
        [SerializeField]
        private FirstPersonCamera _fpsCameraHolder;
        [SerializeField]
        private PlayerUI _playerUI;
        [SerializeField]
        private WeaponController _weapon;

        [SerializeField]
        private GameObject _playerGrave;
        [SerializeField]
        private GameObject _playerOnDeathEffect;

        private Rigidbody _playerRb;
        private Camera _mainCamera;

        public Transform SelfTransform => _selfTransform;
        public Transform Orientation => _orientation;
        public Transform PlayerObj => _playerObj;

        public PlayerUI PlayerUI => _playerUI;

        public Rigidbody PlayerRb => _playerRb;

        public Camera MainCamera => _mainCamera;

        public WeaponController Weapon => _weapon;

        private void Awake()
        {
            _playerRb = GetComponent<Rigidbody>();
            _playerRb.freezeRotation = true;
        }

        private IPlayerController _player;

        public void Init(IPlayerController player, Camera camera)
        {
            _player = player;

            _mainCamera = camera;

            if (_fpsCameraHolder != null)
            {
                if (photonView.IsMine)
                {
                    _fpsCameraHolder.Init();
                    _fpsCameraHolder.SetCamera(_mainCamera);
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            _playerUI.Init(_mainCamera, photonView.Owner.NickName, player.CurrentHealth);

            if (photonView.IsMine)
            {
                _playerUI.Hide();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_player.CurrentHealth);
                stream.SendNext(_player.PlayerLevel);
            }
            else
            {
                _player.CurrentHealth = (float)stream.ReceiveNext();
                _player.PlayerLevel = (int)stream.ReceiveNext();
            }
        }

        public void TakeDamage(DamageData damage)
        {
            if (_player.State == PlayerState.Dead)
                return;

            if (_player.CurrentHealth > 0)
            {
                var resultHealth = _player.CurrentHealth - damage.Value;

                photonView.RPC(nameof(UpdateSelfHealth), RpcTarget.AllViaServer, new object[] { photonView.ViewID, resultHealth });
            }
        }

        public void Death()
        {
            if (_player.State == PlayerState.Dead)
                return;

            var grave
                = PhotonNetwork.Instantiate(_playerGrave.name, transform.position, _orientation.rotation).GetComponent<PlayerGrave>();

            if (photonView.IsMine)
            {
                grave.Init(this);
            }

            photonView.RPC(nameof(OnDeathExecute), RpcTarget.AllViaServer, new object[] { photonView.ViewID, transform.position });
        }

        public void Revive()
        {

            photonView.RPC(nameof(UpdateSelfHealth), RpcTarget.AllViaServer, new object[] { photonView.ViewID, _player.MaxHealth });

            photonView.RPC(nameof(OnReviveExecute), RpcTarget.AllViaServer, new object[] { photonView.ViewID});
        }

        [PunRPC]
        private void UpdateSelfHealth(int id, float value)
        {
            if (photonView.ViewID != id)
                return;

            _player.ChangeHealthValue(value);
        }

        [PunRPC]
        private void OnDeathExecute(int id, Vector3 position)
        {
            if (photonView.ViewID != id)
                return;

            Instantiate(_playerOnDeathEffect, position, Quaternion.identity);

            gameObject.SetActive(false);
        }

        [PunRPC]
        private void OnReviveExecute(int id)
        {
            if (photonView.ViewID != id)
                return;

            gameObject.SetActive(true);

            _player.ChangeState(PlayerState.Alive);
        }

        #region IFindable

        [Header("IFindable Settings")]
        [SerializeField]
        private List<Transform> _visiblePoints;

        bool IFindable.IsAvailable => _player.State == PlayerState.Alive;
        GameObject IFindable.GameObject => gameObject;

        List<Transform> IFindable.VisiblePoints => _visiblePoints;

        Vector3 IFindable.GetPosition()
        {
            return transform.position;
        }

        #endregion
    }
}
