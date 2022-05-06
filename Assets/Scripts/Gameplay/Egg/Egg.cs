using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Photon.Pun;
using Penwyn.Tools;

namespace Penwyn.Game
{
    public class Egg : MonoBehaviourPun
    {
        public EggType Type = EggType.Normal;
    }

    public enum EggType
    {
        Normal,
        Bomb
    }
}

