using System;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class StateTransition : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private GameObject _animatorHolder;
        [SerializeField]
        private float _transitionTime = 0.4f;

        private Action _onEndTransitionCallBack;

        private void Awake()
        {
            _animatorHolder.SetActive(false);
        }

        public void Invoke(Action onEndTransitionCallBack)
        {
            _onEndTransitionCallBack = onEndTransitionCallBack;

            StartCoroutine(Transition());
        }

        IEnumerator Transition()
        {
            _animatorHolder.SetActive(true);

            yield return new WaitForSeconds(_transitionTime);
         
            _animator.SetTrigger("End");

            _onEndTransitionCallBack?.Invoke();

            yield return new WaitForSeconds(_transitionTime);

            _onEndTransitionCallBack = default;

            _animatorHolder.SetActive(false);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            _onEndTransitionCallBack = default;
        }
    }
}
