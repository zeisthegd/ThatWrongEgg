using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Penwyn.Tools;
using System;

namespace Penwyn.Game
{
    public class Character3DRun : CharacterAbility
    {
        [Header("Speeds")]
        public float RunSpeed = 5;
        public float AirSpeed = 5;

        [Header("Raw or Accelerate")]
        public bool UseRawInput = true;
        public float OppositeDirectionPower = 2;
        [Header("Rotation")]
        public RotateType RotateType = RotateType.MoveInput;
        public float TurnSmoothTime = 0.25F;
        public ControlType ControlType;

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
            if (ControlType == ControlType.PlayerInput && AbilityAuthorized)
            {
                if (_controller.IsTouchingGround)
                {
                    if (UseRawInput)
                        RunRaw(InputReader.Instance.MoveInput.normalized);
                    else
                        RunAccelerate(InputReader.Instance.MoveInput.normalized);
                    Debug.Log(_controller.Velocity.magnitude);
                }
                else
                {
                    AirStrafe();
                }
            }
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

        private void Rotate()
        {
            if (RotateType == RotateType.MoveInput)
                RotateByMoveInput();
            else
                RotateByMouse();
        }


        public virtual void RotateByMoveInput()
        {
            if (AbilityAuthorized)
            {
                if (InputReader.Instance.MoveInput.magnitude > 0.01f)
                {
                    float turnSmoothVelocity = 0;
                    Vector2 direction = InputReader.Instance.MoveInput;
                    float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);

                    _character.transform.rotation = Quaternion.Euler(0, angle, 0);
                }
            }
        }


        public virtual void RotateByMouse()
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


    }
    public enum ControlType
    {
        PlayerInput,
        Script
    }

    public enum RotateType
    {
        Mouse,
        MoveInput
    }
}