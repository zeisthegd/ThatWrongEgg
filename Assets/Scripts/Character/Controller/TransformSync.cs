using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Penwyn.Game
{
    public class TransformSync : MonoBehaviourPun, IPunObservable
    {
        public float DistanceToTeleport = 5;
        public bool UpdateTransform = true;
        public float PositionSmoothDampTime = 0.1F;
        public float RotationSmoothDampTime = 0.1F;

        protected Vector3 _remotePosition;
        protected Vector3 _remoteVelocity;
        protected Quaternion _remoteRotation;
        protected Vector3 _remoteEulerAngle;

        protected float _remoteVelocityMagnitude = 0;
        protected float _positionLerpSpeed;
        protected float _rotationLerpSpeed;
        protected CharacterController _controller;

        protected Vector3 _refPosition = Vector3.zero;
        protected Vector3 _refVelocity = Vector3.zero;
        protected Vector3 _refRotationVelocity = Vector3.zero;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }


        void Update()
        {
            if (!photonView.IsMine)
            {
                // Smooth damp the current transform and the remote transform.
                if (UpdateTransform)
                {
                    Vector3 angle = Vector3.SmoothDamp(transform.eulerAngles, _remoteEulerAngle, ref _refRotationVelocity, RotationSmoothDampTime);
                    _controller.transform.rotation = Quaternion.Euler(angle);
                }
            }
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine)
            {
                if (UpdateTransform)
                {
                    _controller.SetPosition(Vector3.SmoothDamp(_controller.transform.position, _remotePosition, ref _refPosition, PositionSmoothDampTime));
                    _controller.SetVelocity(Vector3.SmoothDamp(_controller.Velocity, _remoteVelocity, ref _refVelocity, PositionSmoothDampTime));
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_controller.transform.position);
                stream.SendNext(_controller.Velocity);
                stream.SendNext(transform.rotation);
                stream.SendNext(transform.eulerAngles);
            }

            if (stream.IsReading)
            {
                _remotePosition = (Vector3)stream.ReceiveNext();
                _remoteVelocity = (Vector3)stream.ReceiveNext();
                _remoteRotation = (Quaternion)stream.ReceiveNext();
                _remoteEulerAngle = (Vector3)stream.ReceiveNext();
                _remoteVelocityMagnitude = _remoteVelocity.magnitude;

                if (Vector3.Distance(transform.position, _remotePosition) > DistanceToTeleport)
                {
                    transform.position = _remotePosition;
                }
            }
        }

    }

}
