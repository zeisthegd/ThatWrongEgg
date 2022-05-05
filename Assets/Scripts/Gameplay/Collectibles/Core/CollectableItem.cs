using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;

using Penwyn.Tools;

namespace Penwyn.Game
{

    public class CollectableItem : MonoBehaviourPun
    {
        public List<ItemAction> ActionsOnTouch;
        public LayerMask TargetMask;
        public bool DisableAfterTouch = true;
        public GameObject TouchedObject;

        protected virtual void OnTriggerEnter(Collider col)
        {
            if (TargetMask.Contains(col.gameObject.layer))
            {
                TouchedObject = col.gameObject;
                PerformItemActions();
                if (DisableAfterTouch)
                {
                    photonView.RPC(nameof(RPC_Disable), RpcTarget.All);
                }
                //TODO Play sound.
            }
        }

        public virtual void PerformItemActions()
        {
            foreach (ItemAction action in ActionsOnTouch)
            {
                action.Act(TouchedObject);
            }
        }

        [PunRPC]
        public virtual void RPC_Disable()
        {
            gameObject.SetActive(false);
        }
    }

}