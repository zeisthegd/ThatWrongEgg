using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun.UtilityScripts;
using Photon.Realtime;
namespace Penwyn.Game
{
    [System.Serializable]
    public class TeamData
    {
        public PhotonTeam Team;
        public Player[] Players;
        public int Score;
        public int CurrentDeath;

        public TeamData()
        {
            Team = new PhotonTeam();
            Players = new Player[] { };
            Score = 0;
            CurrentDeath = 0;
        }
    }
}
