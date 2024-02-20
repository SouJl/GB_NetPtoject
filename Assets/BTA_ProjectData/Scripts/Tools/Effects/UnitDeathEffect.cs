﻿using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class UnitDeathEffect : BaseEffect
    {
        [SerializeField]
        private float _destroyDelay = 3f;

        protected override void OnAwake()
        {
            StartCoroutine(DestroyDelay());
        }

        private IEnumerator DestroyDelay()
        {
            yield return new WaitForSeconds(_destroyDelay);

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        
        }
    }
}
