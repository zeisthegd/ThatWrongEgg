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
    public class CombatManager : SingletonMonoBehaviour<CombatManager>
    {
        [ReadOnly] public Player CurrentPlayer;
        protected PhotonView _photonView;


        protected List<TeamData> _teams = new List<TeamData>();

        public event UnityAction TurnChanged;
        public event UnityAction ScoreChanged;

        public virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            InitTeams();
        }

        public virtual void StartGame()
        {
            GetTeams();
            ConnectPlayerEvents();
        }

        public virtual void InitTeams()
        {
            _teams = new List<TeamData>();
            for (int i = 0; i < 4; i++)
            {
                _teams.Add(new TeamData());
            }
            GetTeams();
        }

        public virtual void GetTeams()
        {
            for (int i = 0; i < 4; i++)
            {
                PhotonTeamsManager.Instance.TryGetTeamByCode((byte)i, out _teams[i].Team);
                PhotonTeamsManager.Instance.TryGetTeamMembers((byte)i, out _teams[i].Players);
            }
        }

        public virtual void LocalPlayerDeath(Character player)
        {
            _photonView.RPC(nameof(PlayerDeath), RpcTarget.All, PhotonNetwork.LocalPlayer);
        }

        [PunRPC]
        public virtual void PlayerDeath(Player player)
        {
            HandleAllTeamMemberDeath();
            ScoreChanged?.Invoke();
        }

        protected virtual void HandleAllTeamMemberDeath()
        {
            for (int i = 0; i < 4; i++)
            {
                if (_teams[i].CurrentDeath == _teams[i].Players.Length)
                    GameManager.Instance.LoadNextLevel();

            }
        }

        public virtual void ResetDeathCount()
        {
            for (int i = 0; i < 4; i++)
            {
                _teams[i].CurrentDeath = 0;
            }
        }

        public virtual bool IsSameTeam(Player player)
        {
            if (player.GetPhotonTeam() == PhotonNetwork.LocalPlayer.GetPhotonTeam())
                return true;
            return false;
        }


        public virtual void ConnectPlayerEvents()
        {
            PlayerManager.Instance.LocalPlayer.Health.OnDeath += LocalPlayerDeath;
        }

        public virtual void DisconnectEvents()
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null)
            {
                PlayerManager.Instance.LocalPlayer.Health.OnDeath -= LocalPlayerDeath;
            }
        }

        protected virtual void OnDisable()
        {
            DisconnectEvents();
        }

        public List<TeamData> Teams => _teams;

    }

    public enum Turn
    {
        Self,
        Enemy,
        Teammate
    }
}

