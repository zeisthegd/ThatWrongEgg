using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;

using NaughtyAttributes;

using Penwyn.Tools;
namespace Penwyn.Game
{
    /// <summary>
    /// This should be in every controllable rigidbody.
    /// </summary>
    public class CharacterController : MonoBehaviour
    {
        [Expandable] public PhysicsSettings Settings;
        protected Rigidbody _body;
        protected Collider _collider;

        private StateMachine<ControllerState> _states;

        public Vector3 ExternalForce;
        public float ExternalForceDepleteRate = 1;

        public event UnityAction GroundTouched;
        public event UnityAction WallTouched;

        [HorizontalLine(1, EColor.Green)]
        [ReadOnly] public bool IsTouchingGround;
        [ReadOnly] public bool IsTouchingWall;




        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _states = new StateMachine<ControllerState>(ControllerState.None);
        }

        void Update()
        {
            ExternalForce = Vector3.Lerp(ExternalForce, Vector3.zero, ExternalForceDepleteRate * Time.deltaTime);
        }

        public virtual void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            if (force != null)
                _body.AddForce(force, mode);
        }

        public virtual void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        public virtual void AddPosition(Vector3 positionAddition)
        {
            transform.position += positionAddition;
        }

        public virtual void MultiplyVelocity(float multiplier)
        {
            SetVelocity(_body.velocity * multiplier);
        }

        public virtual void SetVelocity(Vector3 newVelocity)
        {
            _body.velocity = newVelocity + ExternalForce;
        }

        #region Physics Check

        public virtual void CheckOnGround()
        {

        }

        public bool RayCastDoesHit(Collider origin, Vector3 direction, float distance, LayerMask mask)
        {
            float extentsAxis = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? origin.bounds.extents.x : origin.bounds.extents.y;
            //Debug.DrawRay(origin.bounds.center, direction * (extentsAxis + distance), Color.red, 2);
            return Physics.Raycast(origin.bounds.center, direction, extentsAxis + distance, mask);
        }

        public RaycastHit RayCast(Collider origin, Vector3 direction, float distance, LayerMask mask)
        {
            Vector3 halfExtents = new Vector3(direction.x * origin.bounds.extents.x, direction.y * origin.bounds.extents.y, direction.z * origin.bounds.extents.z);
            RaycastHit hit;
            Physics.Raycast(origin.bounds.center, direction, out hit, halfExtents.magnitude + distance, mask);
            return hit;
        }

        public bool BoxCast(Collider origin, float distance, LayerMask mask)
        {
            return Physics.BoxCast(origin.bounds.center, origin.bounds.extents * 3, transform.forward, transform.rotation, distance, mask);
        }

        #endregion

        public virtual void OnCollisionStay(Collision col)
        {
            if (CastForGround())
            {
                IsTouchingGround = true;
                GroundTouched?.Invoke();
            }
            else
            {
                IsTouchingGround = false;
            }

            if (CastForWall())
            {
                IsTouchingWall = true;
                WallTouched?.Invoke();
            }
            else
            {
                IsTouchingWall = false;
            }
        }

        public virtual void OnCollisionExit(Collision col)
        {
            if (Settings.GroundWallMask.Contains(col.gameObject.layer))
            {
                if (IsTouchingGround)
                    IsTouchingGround = CastForGround();
                if (IsTouchingWall)
                    IsTouchingWall = CastForWall();
            }
        }

        public bool CastForGround()
        {
            return RayCastDoesHit(this._collider, Vector3.down, Settings.GroundCastDistance, Settings.GroundWallMask);
        }

        public bool CastForWall()
        {
            return RayCastDoesHit(this._collider, -transform.right, Settings.WallCastDistance, Settings.GroundWallMask)
                || RayCastDoesHit(this._collider, transform.right, Settings.WallCastDistance, Settings.GroundWallMask)
                    || RayCastDoesHit(this._collider, transform.forward, Settings.WallCastDistance, Settings.GroundWallMask)
                        || RayCastDoesHit(this._collider, -transform.forward, Settings.WallCastDistance, Settings.GroundWallMask);
        }


        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        public Rigidbody Body { get => _body; }
        public Collider Collider { get => _collider; set => _collider = value; }
        public Vector3 Velocity { get => _body.velocity; set => SetVelocity(value); }
        public StateMachine<ControllerState> States { get => _states; }
    }
}

