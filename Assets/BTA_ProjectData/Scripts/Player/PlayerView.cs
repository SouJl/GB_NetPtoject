using Photon.Pun;
using Tools;
using UnityEngine;

namespace BTAPlayer
{
    public class PlayerView : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField]
        private Transform _selfTransform;
        [SerializeField]
        private Transform _orientation;
        [SerializeField]
        private PlayerUI _playerUI;

        private Rigidbody _playerRb;
        private Camera _mainCamera;

        public Transform SelfTransform => _selfTransform;
        public Transform Orientation => _orientation;
        public PlayerUI PlayerUI => _playerUI;

        public Rigidbody PlayerRb => _playerRb;

        public Camera MainCamera => _mainCamera;


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

            var followedCamera = GetComponent<FirstPersonCamera>();

            if (followedCamera != null)
            {
                if (photonView.IsMine)
                {
                    followedCamera.Init(_mainCamera);
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

        public void TakeDamage(float damageValue)
        {
            if (_player.CurrentHealth > 0)
            {
                _player.CurrentHealth -= damageValue;

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
    }
}
