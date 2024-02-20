using Photon.Pun;
using UnityEngine;

namespace Tools
{
    public enum GateState
    {
        None,
        Open,
        Close,
    }

    public class GateController : MonoBehaviourPunCallbacks
    {
        private readonly string StateTrigger = "IsOpen";

        [SerializeField]
        private LayerMask _whoCanInteract;
        [SerializeField]
        private Animator _leftDoorAnimator;
        [SerializeField]
        private Animator _rightDoorAnimator;
        [SerializeField]
        private CollisionDetector _collisionDetector;

        private GateState _state;
        private bool _isOppenerExist;

        private void Awake()
        {

            _isOppenerExist = false;
            
            _state = GateState.Close;
        }

        private void Start()
        {
            _collisionDetector.OnEnter += CheckForOpener;
            _collisionDetector.OnExit += OpenerLeftFromGate;
        }

        private void Update()
        {
            if (!_isOppenerExist)
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                photonView.RPC(nameof(Open), RpcTarget.AllViaServer);
            }
        }


        private void CheckForOpener(Collider collider)
        {
            if ((_whoCanInteract & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
            {
                _isOppenerExist = true;
            }
        }
        
        private void OpenerLeftFromGate(Collider collider)
        {
            if (!_isOppenerExist)
                return;

            if ((_whoCanInteract & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
            {
                photonView.RPC(nameof(Close), RpcTarget.AllViaServer);
            }

            _isOppenerExist = false;
        }

        [PunRPC]
        private void Open()
        {
            if (_state == GateState.Open)
                return;

            _leftDoorAnimator.SetBool(StateTrigger, true);
            _rightDoorAnimator.SetBool(StateTrigger, true);

            _state = GateState.Open;
        }

        [PunRPC]
        private void Close()
        {
            if (_state == GateState.Close)
                return;

            _leftDoorAnimator.SetBool(StateTrigger, false);
            _rightDoorAnimator.SetBool(StateTrigger, false);

            _state = GateState.Close;
        }


        private void OnDestroy()
        {
            _collisionDetector.OnEnter -= CheckForOpener;
        }
    }
}
