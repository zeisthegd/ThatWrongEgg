using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Penwyn.Tools;

namespace Penwyn.Game
{
    public class CharacterGrab : CharacterAbility
    {
        public Transform GrabTransform;
        public float GrabRange = 2;
        public LayerMask TargetMask;
        public LayerMask ObstacleMask;


        protected List<GrabableObject> _targetList;
        public override void AwakeAbility(Character character)
        {
            base.AwakeAbility(character);
            _targetList = new List<GrabableObject>();
        }

        public virtual void Grab()
        {
            if (AbilityAuthorized)
            {
                FindTargetInRange();
                foreach (GrabableObject grabableObject in _targetList)
                {
                    grabableObject.RPC_GetGrabbed(_character.photonView.OwnerActorNr);
                }
            }
        }

        public void FindTargetInRange()
        {
            var _detectedColliders = Physics.OverlapSphere(_character.Position, GrabRange, TargetMask);
            _targetList.Clear();
            foreach (Collider collider in _detectedColliders)
            {
                if (!Physics.Raycast(_character.Position, collider.transform.position - _character.Position, Vector3.Distance(collider.transform.position, _character.Position), ObstacleMask))
                {
                    float angleToTarget = Vector3.SignedAngle(_character.transform.forward, collider.transform.position - _character.transform.position, Vector3.up);
                    if (collider.GetComponent<GrabableObject>() && angleToTarget >= -90 && angleToTarget <= 90)
                        _targetList.Add(collider.GetComponent<GrabableObject>());
                }
            }
            if (_targetList.Count > 0)
            {
                _targetList = _targetList.OrderBy(x => Vector3.Distance(x.transform.position, _character.Position)).ToList();
            }
        }

        public override void ConnectEvents()
        {
            base.ConnectEvents();
            InputReader.Instance.ItemPressed += Grab;
        }

        public override void DisconnectEvents()
        {
            base.DisconnectEvents();
            InputReader.Instance.ItemPressed -= Grab;
        }
    }
}
