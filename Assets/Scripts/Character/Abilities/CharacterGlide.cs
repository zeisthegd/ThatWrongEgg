using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Penwyn.Tools;
namespace Penwyn.Game
{
    public class CharacterGlide : CharacterAbility
    {
        [Header("Dive")]
        public float MaxForce = 30;
        public float AirStrafeForce = 10;
        public float MinForcePercentage = 0.5F;

        [Header("Levitate")]
        public float LevitateForceMultiplier = 1.5F;
        public float LevitateForceDecreaseMultiplier = 1.5F;
        public float LevitateExternalForce = 1;

        [Header("VFX")]
        public float CameraDistanceWhenDiving = 10;
        public float MinFOV = 40;
        public float MaxFOV = 70;
        public float FOVAdjustDuration = 1;
        public FeedbackCameraShakeData FlyingShakeData;


        protected float _forcePercentage = 100;
        protected bool _isGliding = false;
        protected float _maxSpeed = 0;
        protected float _topRecordedVelocity = 0;
        protected float _fov = 0;
        protected float _camDst = 0;

        public override void AwakeAbility(Character character)
        {
            base.AwakeAbility(character);
        }

        public override void UpdateAbility()
        {
            base.UpdateAbility();
            if (AbilityAuthorized)
            {
                CameraShake();
                AdjustFlightFOV();
                AdjustFlightCamDistance();
            }
        }

        public override void FixedUpdateAbility()
        {
            base.FixedUpdateAbility();
            if (AbilityAuthorized)
                Glide();
        }

        public virtual void StartGliding()
        {
            if (CanStartGliding && AbilityAuthorized)
            {
                _isGliding = true;
            }
        }
        public virtual void StopGliding()
        {
            if (_isGliding && AbilityAuthorized)
            {
                Reset();
            }
        }
        public virtual void Glide()
        {
            if (InputReader.Instance.IsHoldingGlide && !_controller.IsTouchingGround)
            {
                float camAngle = Camera.main.transform.eulerAngles.x > 180 ? 360 - Camera.main.transform.eulerAngles.x : Camera.main.transform.eulerAngles.x;
                _forcePercentage = Mathf.Abs(camAngle / 90);
                _forcePercentage = Mathf.Clamp(_forcePercentage, MinForcePercentage, Mathf.Abs(camAngle / 90));
                _controller.Body.useGravity = IsLookingUp;

                if (IsLookingUp)
                    Levitate();
                else
                    Dive();
                AddHelpingLevitateForce();
                _character.Model.transform.localRotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x + 90, 0, 0);
            }
            else
            {
                StopGliding();
            }
        }

        public virtual void Levitate()
        {
            if (_topRecordedVelocity > 1)
                _controller.SetVelocity(Camera.main.transform.forward * _topRecordedVelocity * LevitateForceMultiplier);
            _topRecordedVelocity = Mathf.Clamp(_topRecordedVelocity - Time.deltaTime * LevitateForceDecreaseMultiplier, 0, _topRecordedVelocity - Time.deltaTime * LevitateForceDecreaseMultiplier);

        }

        public virtual void Dive()
        {
            _maxSpeed = _forcePercentage * MaxForce;
            _controller.SetVelocity(_maxSpeed * Camera.main.transform.forward);
            if (_controller.Velocity.y < 0 && _topRecordedVelocity < Mathf.Abs(_controller.Velocity.y))
            {
                _topRecordedVelocity = Mathf.Abs(_controller.Velocity.y);
            }
        }

        public virtual void AddHelpingLevitateForce()
        {
            if (InputReader.Instance.IsHoldingJump)
            {
                _controller.SetVelocity(Camera.main.transform.forward * LevitateExternalForce);
            }
        }

        public virtual void AirStrafe()
        {
            if (_controller.Velocity.z < AirStrafeForce)
            {
                _controller.AddForce(_character.transform.right * InputReader.Instance.MoveInput.y * AirStrafeForce, ForceMode.Impulse);
                Debug.DrawRay(_character.Position, _character.transform.right * InputReader.Instance.MoveInput.y * AirStrafeForce, Color.red, 2);
            }
        }

        public virtual void AdjustFlightFOV()
        {
            if (_isGliding)
            {
                _fov += Time.deltaTime * (MaxFOV - MinFOV) * FOVAdjustDuration;
            }
            else
            {
                _fov -= Time.deltaTime * (MaxFOV - MinFOV) * FOVAdjustDuration;
            }
            _fov = Mathf.Clamp(_fov, MinFOV, MaxFOV);
            CameraManager.Instance.CurrenPlayerCam.SetFOV(_fov);
        }

        public virtual void AdjustFlightCamDistance()
        {
            if (_isGliding)
            {
                _camDst += Time.deltaTime * (MaxFOV - MinFOV) * FOVAdjustDuration;
            }
            else
            {
                _camDst -= Time.deltaTime * (MaxFOV - MinFOV) * FOVAdjustDuration;
            }
            _camDst = Mathf.Clamp(_camDst, 0.01f, CameraDistanceWhenDiving);
            CameraManager.Instance.CurrenPlayerCam.ChangeBodyDistance(_camDst);
        }


        public virtual void CameraShake()
        {
            if (_isGliding)
                CameraManager.Instance.CurrenPlayerCam.StartShaking(FlyingShakeData.Amplitude * _forcePercentage, FlyingShakeData.Frequency * _forcePercentage);
        }
        public virtual void Reset()
        {
            _forcePercentage = 1;
            _fov = 40;
            _isGliding = false;
            _maxSpeed = 0;
            _topRecordedVelocity = 0;
            _controller.Body.useGravity = true;
            CameraManager.Instance.CurrenPlayerCam.StartShaking(0, 0);
        }

        public override void ConnectEvents()
        {
            if (AbilityAuthorized)
            {
                base.ConnectEvents();
                InputReader.Instance.GlidePressed += StartGliding;
                InputReader.Instance.GlideReleased += StopGliding;
                _controller.GroundTouched += StopGliding;
            }
        }

        public override void DisconnectEvents()
        {
            if (AbilityAuthorized)
            {
                base.DisconnectEvents();
                InputReader.Instance.GlidePressed -= StartGliding;
                InputReader.Instance.GlideReleased -= StopGliding;
                _controller.GroundTouched -= StopGliding;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public bool CanStartGliding => AbilityPermitted && !_controller.IsTouchingGround;

        public bool IsLookingUp => Camera.main.transform.eulerAngles.x > 180;
    }

}