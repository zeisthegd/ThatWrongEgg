using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Penwyn.Tools;
namespace Penwyn.Game
{
    public class CharacterJump : CharacterAbility
    {
        public float MaxJumpForce = 30;
        public float ReleaseEarlyForce = 2;

        public virtual void Jump()
        {
            if (AbilityAuthorized && _controller.IsTouchingGround)
            {
                _controller.AddForce(Vector3.up * MaxJumpForce, ForceMode.VelocityChange);
                _controller.AddForce(Camera.main.transform.forward, ForceMode.VelocityChange);
            }
        }

        public virtual void AddReleaseEarlyForce()
        {
            if (AbilityAuthorized)
            {
                if (_controller.Velocity.y > 0)
                {
                    _controller.AddForce(Vector3.down * ReleaseEarlyForce, ForceMode.VelocityChange);
                }
            }
        }

        public override void ConnectEvents()
        {
            if (AbilityAuthorized)
            {
                base.ConnectEvents();
                InputReader.Instance.JumpPressed += Jump;
                InputReader.Instance.JumpReleased += AddReleaseEarlyForce;
            }
        }

        public override void DisconnectEvents()
        {
            if (AbilityAuthorized)
            {
                base.DisconnectEvents();
                InputReader.Instance.JumpPressed -= Jump;
                InputReader.Instance.JumpReleased -= AddReleaseEarlyForce;
            }
        }
    }

}
