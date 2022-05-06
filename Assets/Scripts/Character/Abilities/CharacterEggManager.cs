using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Photon;
using Photon.Pun;

using Penwyn.Tools;

namespace Penwyn.Game
{
    public class CharacterEggManager : CharacterAbility
    {
        public string EggPath;
        public Material NormalSkinMaterial;
        public Material BombSkinMaterial;

        public Vector3 SpawnOffset = new Vector3(0, 1, 0);

        [ReadOnly] public Egg Egg;


        public virtual void CreateNetworkedEgg()
        {
            Egg = PhotonNetwork.Instantiate(EggPath, _character.transform.position + _character.transform.forward + SpawnOffset, Quaternion.identity).GetComponent<Egg>();
        }


        public virtual void EnableEgg()
        {

        }
    }
}
