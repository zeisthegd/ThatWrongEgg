using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Penwyn.Game
{
    public class TransformSync : MonoBehaviourPun, IPunObservable
    {
        public float DistanceToTeleport = 5;
        public AnimationCurve VelocityMagnitudeToLerpSpeedCurve;

        protected Vector3 _remotePosition;
        protected Vector3 _remoteVelocity;
        protected Quaternion _remoteRotation;
        protected Vector3 _remoteEulerAngle;

        protected float _remoteVelocityMagnitude = 0;
        protected float _positionLerpSpeed;
        protected float _rotationLerpSpeed;
        protected CharacterController _controller;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                _controller.SetPosition(Vector3.Lerp(transform.position, _remotePosition, _positionLerpSpeed * Time.unscaledDeltaTime));
                transform.rotation = Quaternion.Lerp(transform.rotation, _remoteRotation, _rotationLerpSpeed * Time.unscaledDeltaTime);
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

                float velocityDifference = Mathf.Abs(_remoteVelocityMagnitude - _controller.Velocity.magnitude);
                float angleDifference = Vector3.Angle(_remoteEulerAngle, _controller.transform.eulerAngles);

                _positionLerpSpeed = VelocityMagnitudeToLerpSpeedCurve.Evaluate(velocityDifference);
                _rotationLerpSpeed = VelocityMagnitudeToLerpSpeedCurve.Evaluate(Mathf.Abs(angleDifference));

                if (Vector3.Distance(transform.position, _remotePosition) > _positionLerpSpeed)
                {
                    transform.position = _remotePosition;
                }
            }
        }

    }

}
