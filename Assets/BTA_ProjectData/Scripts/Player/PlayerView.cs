using Abstraction;
using Photon.Pun;
using System.Collections.Generic;
using Tools;
using UnityEditor;
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
        private FirstPersonCamera _fpsCameraHolder;
        [SerializeField]
        private PlayerUI _playerUI;
        [SerializeField]
        private WeaponController _weapon;

        private Rigidbody _playerRb;
        private Camera _mainCamera;

        public Transform SelfTransform => _selfTransform;
        public Transform Orientation => _orientation;
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

        internal void Init(IPlayerController player, Camera camera)
        {
            _player = player;

            _mainCamera = camera;

            if (_fpsCameraHolder != null)
            {
                if (photonView.IsMine)
                {
                    _fpsCameraHolder.Init(_mainCamera);
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
            if (_player.CurrentHealth > 0)
            {
                _player.CurrentHealth -= damage.Value;

                _playerRb.AddForce(damage.Force);

                photonView.RPC(nameof(UpdateSelfHealth), RpcTarget.Others, new object[] { photonView.ViewID, _player.CurrentHealth });
            }
        }


        [PunRPC]
        private void UpdateSelfHealth(int id, float value)
        {
            if (photonView.ViewID != id)
                return;

            Debug.Log($"Health : {value}");

            _player.ChangeHealthValue(value);
        }

        #region IFindable

        [Header("IFindable Settings")]
        [SerializeField]
        private List<Transform> _visiblePoints;

        Transform IFindable.Transform => transform;

        GameObject IFindable.GameObject => gameObject;

        List<Transform> IFindable.VisiblePoints => _visiblePoints;

        #endregion
    }
}
