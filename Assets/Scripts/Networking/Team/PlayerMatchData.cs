using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun.UtilityScripts;
using Photon.Realtime;
namespace Penwyn.Game
{
    [System.Serializable]
    public class PlayerMatchData
    {
        public int Score;
        public bool EggDestroyed;

        public PlayerMatchData()
        {
            Score = 0;
            EggDestroyed = false;
        }
    }
}
