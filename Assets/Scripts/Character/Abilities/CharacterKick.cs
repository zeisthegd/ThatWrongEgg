using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Penwyn.Tools;
namespace Penwyn.Game
{
    public class CharacterKick : CharacterAbility
    {
        public float KickForce = 10;
        public float KickRange = 2;
        public float KickAngle = 180;//Max angle in front of the character.
        public LayerMask TargetMask;
        public LayerMask ObstacleMask;

        public KickableObject Target;

        protected List<KickableObject> _targetList;

        public override void AwakeAbility(Character character)
        {
            base.AwakeAbility(character);
            _targetList = new List<KickableObject>();
        }

        public virtual void Kick()
        {
            if (AbilityAuthorized)
            {
                FindTargetInRange();
                foreach (KickableObject kickableObject in _targetList)
                {
                    kickableObject.RPC_GetKicked(KickForce * (kickableObject.transform.position - _character.Position).normalized, _character.Position);
                }
            }
        }

        public void FindTargetInRange()
        {
            var _detectedColliders = Physics.OverlapSphere(_character.Position, KickRange, TargetMask);
            _targetList.Clear();
            foreach (Collider collider in _detectedColliders)
            {
                if (!Physics.Raycast(_character.Position, collider.transform.position - _character.Position, Vector3.Distance(collider.transform.position, _character.Position), ObstacleMask))
                {
                    float angleToTarget = Vector3.SignedAngle(_character.transform.forward, collider.transform.position - _character.transform.position, Vector3.up);
                    if (collider.GetComponent<KickableObject>() && angleToTarget >= -90 && angleToTarget <= 90)
                        _targetList.Add(collider.GetComponent<KickableObject>());
                }
            }
            if (_targetList.Count > 0)
            {
                _targetList = _targetList.OrderBy(x => Vector3.Distance(x.transform.position, _character.Position)).ToList();
            }
        }

        public override void ConnectEvents()
        {
            if (AbilityAuthorized)
            {
                base.ConnectEvents();
                InputReader.Instance.KickPressed += Kick;
            }
        }

        public override void DisconnectEvents()
        {
            if (AbilityAuthorized)
            {
                base.DisconnectEvents();
                InputReader.Instance.KickPressed -= Kick;
            }
        }

        void OnDrawGizmos()
        {
            if (_character)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_character.Position, KickRange);
            }
        }
    }

}
