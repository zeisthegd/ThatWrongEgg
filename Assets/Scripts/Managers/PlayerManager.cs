using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

using NaughtyAttributes;
using Penwyn.Tools;

namespace Penwyn.Game
{
    [CreateAssetMenu(menuName = "Managers/Player Manager")]
    public class PlayerManager : SingletonScriptableObject<PlayerManager>
    {
        [Header("Player")]
        public string PlayerPrefabPath;
        //Player Lists
        protected List<Character> _playersInRoom;
        [ReadOnly] public Character LocalPlayer;

        public event UnityAction PlayerSpawned;

        /// <summary>
        /// Instantiate the player model when they enter the room.
        /// </summary>
        public void CreateLocalPlayer()
        {
            if (PhotonNetwork.InRoom && LocalPlayer == null)
            {
                Transform spawnPos = GameObject.Find("SpawnPos").transform;
                GameObject player = PhotonNetwork.Instantiate(PlayerPrefabPath, spawnPos.position, Quaternion.identity);
                player.name = PhotonNetwork.NickName;
                LocalPlayer = player.FindComponent<Character>();
                PlayerSpawned?.Invoke();
                Debug.Log("Player Created!");
                TryJoiningATeam();
            }
            FindPlayersInRooms();
        }

        public virtual void TryJoiningATeam()
        {
            foreach (TeamData team in CombatManager.Instance.Teams)
            {
                Debug.Log(team.Team.Name);
                if (team.Players == null || team.Players.Length == 0)
                {
                    PhotonNetwork.LocalPlayer.JoinTeam(team.Team);
                    Debug.Log($"Joined: {team.Team.Name}");
                    return;
                }
            }
        }

        public void FindPlayersInRooms()
        {
            var playerObjects = GameObject.FindGameObjectsWithTag("Player");
            _playersInRoom = new List<Character>();
            foreach (GameObject playerObj in playerObjects)
            {
                _playersInRoom.Add(playerObj.GetComponent<Character>());
            }
        }

        #region Utilities

        /// <summary>
        /// Find a player by their photonview's owner actor number.
        /// </summary>
        public Character FindByOwnerActorNumber(int ownerActNr)
        {
            FindPlayersInRooms();
            return _playersInRoom.Find(player => player.photonView.OwnerActorNr == ownerActNr);
        }

        #endregion

        void OnEnable()
        {

        }

        public List<Character> PlayerInRoom { get => _playersInRoom; }
    }
}

