using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Photon.Pun;
using Penwyn.Tools;

namespace Penwyn.Game
{
    public class Egg : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        public EggType Type = EggType.Normal;
        public Character Owner;
        public ParticleSystem TypeChangedParticles;

        public const string EggMaterialFolderPath = "Mats&Shaders/Materials/Eggs/";

        public virtual void RPC_ChangeType(EggType newType)
        {
            photonView.RPC(nameof(ChangeType), RpcTarget.All, new object[] { newType });
        }

        [PunRPC]
        protected virtual void ChangeType(EggType newType)
        {
            Type = newType;
            string matName = Type == EggType.Normal ? Owner.CharacterEggManager.NormalSkinMaterial.name : Owner.CharacterEggManager.BombSkinMaterial.name;
            this.gameObject.SetActive(true);
            this.transform.position = Owner.transform.position + Owner.transform.forward;
            this.transform.rotation = Quaternion.identity;

            GetComponent<MeshRenderer>().material = Resources.Load(EggMaterialFolderPath + matName, typeof(Material)) as Material;
            TypeChangedParticles?.Play();

            Debug.Log($"{Owner.name}|{this.Type}|{GetComponent<MeshRenderer>().material.name}");
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            this.Owner = PlayerManager.Instance.FindByOwnerActorNumber(info.Sender.ActorNumber);
            Owner.CharacterEggManager.Egg = this;
            this.gameObject.SetActive(false);
        }
    }

    public enum EggType
    {
        Normal,
        Bomb
    }
}

