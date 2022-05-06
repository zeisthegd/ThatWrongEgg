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
        public LayerMask TargetMask;
        public LayerMask ObstacleMask;

        public KickableObject Target;

        protected List<Transform> _targetList;


        public virtual void Kick()
        {
            if (AbilityAuthorized)
            {
                FindTargetInRange();
                if (Target != null)
                {
                    Target.RPC_GetKicked(KickForce * (Target.transform.position - _character.Position).normalized, _character.Position);
                    Target = null;
                }
            }
        }

        public void FindTargetInRange()
        {
            var _detectedColliders = Physics.OverlapSphere(_character.Position, KickRange, TargetMask);
            _targetList = new List<Transform>();
            foreach (Collider collider in _detectedColliders)
            {
                if (!Physics.Raycast(_character.Position, collider.transform.position - _character.Position, Vector3.Distance(collider.transform.position, _character.Position), ObstacleMask))
                {
                    _targetList.Add(collider.transform);
                }
            }
            if (_targetList.Count > 0)
            {
                _targetList = _targetList.OrderBy(x => Vector3.Distance(x.transform.position, _character.Position)).ToList();
                Target = _targetList[0].GetComponent<KickableObject>();
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
                Gizmos.DrawSphere(_character.Position, KickRange);
            }
        }
    }

}
