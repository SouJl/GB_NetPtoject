using Photon.Pun;
using System;

namespace Tools
{
    public abstract class BaseEffect : MonoBehaviourPunCallbacks
    {
        private void Awake()
        {
            OnAwake();
        }

        protected abstract void OnAwake();
    }
}
