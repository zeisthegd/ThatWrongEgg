using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Photon.Pun;
using Penwyn.Tools;

namespace Penwyn.Game
{
    public class KickableObject : MonoBehaviourPun
    {
        public float MinRemoteKickedDistance = 1;// To reduce lag cheating.
        protected CharacterController _controller;

        protected virtual void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Add an impulsive force to the object via RPC/AllViaServer.
        /// </summary>
        public virtual void RPC_GetKicked(Vector3 force, Vector3 kickedPosition)
        {
            photonView.RPC(nameof(GetKicked), RpcTarget.All, new object[] { force, kickedPosition });
        }

        /// <summary>
        /// Add an impulsive force to the object.
        /// </summary>
        [PunRPC]
        public virtual void GetKicked(Vector3 force, Vector3 kickedPosition)
        {
            if (Vector3.Distance(this.transform.position, kickedPosition) < MinRemoteKickedDistance)
            {
                _controller.AddForce(force, ForceMode.Impulse);
            }
            else
            {
                Debug.Log("Too lagged. Object has moved to another position!");
            }
        }
    }
}

