using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Penwyn.Tools;
using NaughtyAttributes;
namespace Penwyn.Game
{
    public class PushPlane : MonoBehaviour
    {
        [InfoBox("Add more velocity to the object's passing through this. ForceMode")]
        public LayerMask TargetMask;
        public float Multiplier = 0.5F;
        public bool ShouldPushNonClient = false;

        public Vector3 NormalForTriggerBounce;

        public Vector3 FixedForce;
        public PushMode PushMode = PushMode.ObjectVelocity;
        public ForceMode ForceMode = ForceMode.Impulse;

        protected virtual void OnTriggerEnter(Collider col)
        {
            if (TargetMask.Contains(col.gameObject.layer) && col.gameObject.GetComponent<CharacterController>())
            {
                CharacterController controller = col.gameObject.GetComponent<CharacterController>();
                if (ShouldPushObject(controller))
                {
                    PushByObjectVelocity(controller);
                    BounceObject(controller, NormalForTriggerBounce);
                    PushByFixedForce(controller);
                    PushObjectAway(controller);
                }
            }
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
            if (TargetMask.Contains(col.gameObject.layer) && col.gameObject.GetComponent<CharacterController>())
            {
                CharacterController controller = col.gameObject.GetComponent<CharacterController>();
                if (ShouldPushObject(controller))
                {
                    PushByObjectVelocity(controller);
                    BounceObject(controller, col.contacts[0].normal);
                    PushByFixedForce(controller);
                    PushObjectAway(controller);
                }
            }
        }

        protected virtual void PushByObjectVelocity(CharacterController controller)
        {
            if (PushMode == PushMode.ObjectVelocity)
            {
                controller.ExternalForce = controller.Velocity * Multiplier;
            }
        }
        protected virtual void PushByFixedForce(CharacterController controller)
        {
            if (PushMode == PushMode.FixedForce)
            {
                controller.AddForce(FixedForce * Multiplier, ForceMode);
            }
        }
        protected virtual void BounceObject(CharacterController controller, Vector3 normal)
        {
            if (PushMode == PushMode.Bounce)
            {
                var direction = Vector3.Reflect(controller.Velocity.normalized, normal);
                controller.SetVelocity(direction * controller.Velocity.magnitude * Multiplier);
                Debug.DrawRay(controller.transform.position, direction, Color.black, 1);
            }
        }

        protected virtual void PushObjectAway(CharacterController controller)
        {
            if (PushMode == PushMode.Away)
            {
                Vector3 direction = (controller.transform.position - this.transform.position).normalized;
                controller.AddForce(direction * Multiplier, ForceMode);
                Debug.DrawRay(controller.transform.position, direction, Color.black, 1);
            }
        }

        protected virtual Vector3 GetTriggerCollisionBounceNormal(CharacterController controller)
        {
            Vector3 normal = (Mathf.Abs(controller.Velocity.x) > Mathf.Abs(controller.Velocity.z) ? Vector3.right : Vector3.forward);
            return normal;
        }

        protected bool ShouldPushObject(CharacterController controller)
        {
            if ((controller.GetComponent<PhotonView>() != null && controller.GetComponent<PhotonView>().IsMine == false) && ShouldPushNonClient == false)
                return false;
            return true;
        }
    }

    public enum PushMode
    {
        ObjectVelocity,
        FixedForce,
        Away,
        Bounce
    }
}

