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
        public GameObject LocalPlayerMarker;
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
                Instantiate(LocalPlayerMarker, player.transform.position, Quaternion.Euler(-90, 0, 0), player.transform);
                TryJoiningATeam();
            }
            FindPlayersInRooms();
        }

        public virtual void TryJoiningATeam()
        {
            PhotonTeam teamOne;
            PhotonTeam teamTwo;
            int teamOneCount = PhotonTeamsManager.Instance.GetTeamMembersCount(1);
            int teamTwoCount = PhotonTeamsManager.Instance.GetTeamMembersCount(2);

            PhotonTeamsManager.Instance.TryGetTeamByCode(1, out teamOne);
            PhotonTeamsManager.Instance.TryGetTeamByCode(2, out teamTwo);

            if (teamOneCount > teamTwoCount)
                PhotonNetwork.LocalPlayer.JoinTeam(teamTwo);
            else
                PhotonNetwork.LocalPlayer.JoinTeam(teamOne);
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

        public bool LocalPlayerIsFirstTeam => PhotonNetwork.LocalPlayer.GetPhotonTeam() == CombatManager.Instance.FirstTeam.Team;
        public bool LocalPlayerIsSecondTeam => PhotonNetwork.LocalPlayer.GetPhotonTeam() == CombatManager.Instance.SecondTeam.Team;

        public List<Character> PlayerInRoom { get => _playersInRoom; }
    }
}

