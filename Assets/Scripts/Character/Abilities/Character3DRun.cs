using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Penwyn.Tools;

namespace Penwyn.Game
{
    public class Character3DRun : CharacterAbility
    {
        [Header("Speed")]
        public float RunSpeed = 5;
        public float MaxSpeed = 30;
        public float OppositeDirectionPower = 2;
        public float AirSpeed = 5;
        public bool UseRawInput = true;
        public float EnergyPerSecond = 1;
        public ControlType Type;
        [Header("Feedbacks")]
        public ParticleSystem Dust;

        public override void AwakeAbility(Character character)
        {
            base.AwakeAbility(character);
        }

        public override void UpdateAbility()
        {
            base.UpdateAbility();
            Rotate();
        }

        public override void FixedUpdateAbility()
        {
            if (Type == ControlType.PlayerInput && AbilityAuthorized)
            {
                if (_controller.IsTouchingGround)
                {
                    if (InputReader.Instance.MoveInput.magnitude > 0)
                        _character.Energy.Use(EnergyPerSecond * Time.deltaTime, true);
                    if (UseRawInput)
                        RunRaw(InputReader.Instance.MoveInput.normalized);
                    else
                        RunAccelerate(InputReader.Instance.MoveInput.normalized);
                }
                else
                {
                    AirStrafe();
                }
            }
            DustHandling();
        }

        public virtual void RunRaw(Vector2 input)
        {
            Vector3 direction = Vector3.right * input.x + Vector3.forward * input.y;
            if (input.magnitude > 0)
                _controller.SetVelocity(direction * RunSpeed * Time.deltaTime);
        }

        public virtual void RunAccelerate(Vector2 input)
        {
            if (AbilityAuthorized)
            {
                float oppositeDirectionPower = Vector3.Angle(input, _controller.Velocity) > 135 ? OppositeDirectionPower : 1;
                Vector3 direction = Vector3.right * input.x + Vector3.forward * input.y;
                _controller.AddForce(direction * RunSpeed * oppositeDirectionPower * Time.deltaTime, ForceMode.Impulse);
            }
        }

        public virtual void AirStrafe()
        {
            if (AbilityAuthorized)
            {
                Vector3 direction = Vector3.right * InputReader.Instance.MoveInput.x + Vector3.forward * InputReader.Instance.MoveInput.y;
                _controller.AddForce(direction * AirSpeed, ForceMode.Force);
            }
        }

        public virtual void Rotate()
        {
            if (AbilityAuthorized)
            {
                RaycastHit hit = CursorManager.Instance.GetRayHitUnderMouse();
                Vector3 direction = (hit.point - _character.transform.position).normalized;
                direction = Vector3.ProjectOnPlane(direction, Vector3.up);// Ignore the X rotation.
                Debug.DrawRay(_character.transform.position, direction * 1000, Color.green);
                if (-direction != Vector3.zero)
                    _character.transform.forward = direction;
            }
        }



        protected virtual void DustHandling()
        {
            if (Dust != null)
            {
                if (_controller.Velocity.magnitude > 0)
                {
                    if (!Dust.isPlaying)
                        Dust.Play();
                }
                else
                {
                    Dust.Stop();
                }
            }
        }

        public override void ConnectEvents()
        {
            base.ConnectEvents();
        }

        public override void DisconnectEvents()
        {
            base.DisconnectEvents();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public enum ControlType
        {
            PlayerInput,
            Script
        }
    }

}