using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
namespace Penwyn.Game
{
    public class GrabableObject : MonoBehaviourPun
    {
        protected GrabbedState _state = GrabbedState.NotGrabbed;

        public virtual void Update()
        {
            if (_state == GrabbedState.Grabbed)
            {
                transform.position = transform.parent.position;
                transform.localRotation = Quaternion.identity;
            }
        }
        public virtual void RPC_GetGrabbed(int ownerActorNr)
        {
            photonView.RPC(nameof(GetGrabbed), RpcTarget.All, ownerActorNr);
        }

        [PunRPC]
        public virtual void GetGrabbed(int ownerActorNr)
        {
            Character player = PlayerManager.Instance.FindByOwnerActorNumber(ownerActorNr);
            Debug.Log(player.name);
            this.transform.SetParent(player.FindAbility<CharacterGrab>().GrabTransform);
            this.transform.position = player.FindAbility<CharacterGrab>().GrabTransform.position;
            _state = GrabbedState.Grabbed;
        }
    }

    public enum GrabbedState
    {
        NotGrabbed,
        Grabbed
    }

}
