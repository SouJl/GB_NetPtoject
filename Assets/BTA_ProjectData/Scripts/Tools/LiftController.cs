using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class LiftController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _stopDelay = 1.5f;
        [SerializeField]
        private int _startPoint;
        [SerializeField]
        private Transform[] _points;

        private bool _isStoped;

        private int _pointIndex;
        private bool _isReverse;


        private void Start()
        {

            transform.position = _points[_startPoint].position;
            _pointIndex = _startPoint;

            _isStoped = false;
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if(Vector3.Distance(transform.position, _points[_pointIndex].position) < 0.01f)
            {
                if(_pointIndex == _points.Length - 1)
                {
                    _isReverse = true;
                    _pointIndex--;

                    StartCoroutine(OnEdgeDelay());

                    return;
                }
                else if(_pointIndex == 0)
                {
                    _isReverse = false;
                    _pointIndex++;

                    StartCoroutine(OnEdgeDelay());

                    return;
                }

                _pointIndex += _isReverse ? -1 : 1;

            }

            if (!_isStoped)
            {
                transform.position
                    = Vector3.MoveTowards(transform.position, _points[_pointIndex].position, _speed * Time.deltaTime);
            }
        }

        private IEnumerator OnEdgeDelay()
        {
            _isStoped = true;

            yield return new WaitForSeconds(_stopDelay);

            _isStoped = false;
        }
    }
}
