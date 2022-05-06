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
        public Vector3 SpawnOffset = new Vector3(0, 1, 0);

        [SerializeField][ReadOnly] protected Egg _egg;

        public virtual void CreateNetworkedEgg()
        {
            _egg = PhotonNetwork.Instantiate(EggPath, _character.transform.position + _character.transform.forward + SpawnOffset, Quaternion.identity).GetComponent<Egg>();
        }

        public virtual void EnableEgg()
        {

        }
    }
}
