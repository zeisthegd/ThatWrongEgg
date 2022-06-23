using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;
using Photon.Pun;
using Penwyn.Tools;

namespace Penwyn.Game
{
    public class GolfBall : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        public const string MaterialFolderPath = "Mats&Shaders/Materials/Eggs/";
        public StateMachine<GolfBallState> State;

        public event UnityAction<Character> PlayerTouched;


        protected virtual void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag == "Player")
            {
                PlayerTouched?.Invoke(col.gameObject.GetComponent<Character>());
            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient == false)
            {
                CombatManager.Instance.AssignBall(this);
            }
        }

    }

    public enum GolfBallState
    {
        Moving,
        Stopped
    }
}

