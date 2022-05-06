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
        [Header("General")]
        public EggType Type = EggType.Normal;
        public Character Owner;
        public Health Health;
        public ParticleSystem TypeChangedParticles;

        [Header("Bomb - Explosion")]
        public DamageOnTouch DamageZone;
        public ParticleSystem ExplosionParticles;

        public const string EggMaterialFolderPath = "Mats&Shaders/Materials/Eggs/";

        void Awake()
        {
            Health = GetComponent<Health>();
        }
        public virtual void Explode()
        {
            DamageZone.gameObject.SetActive(true);
            ExplosionParticles?.Play();
        }

        public virtual void RPC_ChangeType(EggType newType)
        {
            photonView.RPC(nameof(ChangeType), RpcTarget.All, new object[] { newType });
        }

        [PunRPC]
        protected virtual void ChangeType(EggType newType)
        {
            StopAllParticles();
            Type = newType;
            string matName = Type == EggType.Normal ? Owner.CharacterEggManager.NormalSkinMaterial.name : Owner.CharacterEggManager.BombSkinMaterial.name;
            this.gameObject.SetActive(true);
            this.transform.position = Owner.transform.position + Owner.transform.forward;
            this.transform.rotation = Quaternion.identity;

            GetComponent<MeshRenderer>().material = Resources.Load(EggMaterialFolderPath + matName, typeof(Material)) as Material;
            TypeChangedParticles?.Play();

            DamageZone.gameObject.SetActive(false);
            Debug.Log($"{Owner.name}|{this.Type}|{GetComponent<MeshRenderer>().material.name}");
        }

        public virtual void StopAllParticles()
        {
            ExplosionParticles?.Stop();
            TypeChangedParticles?.Stop();
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

