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


        protected List<PlayerMatchData> _playerMatchDataList = new List<PlayerMatchData>();

        public event UnityAction TurnChanged;
        public event UnityAction ScoreChanged;

        public virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            PlayerMatchDatas();
        }

        public virtual void StartGame()
        {
            ConnectPlayerEvents();
        }

        public virtual void PlayerMatchDatas()
        {
            _playerMatchDataList = new List<PlayerMatchData>();
            for (int i = 0; i < 4; i++)
            {
                _playerMatchDataList.Add(new PlayerMatchData());
            }
        }

        #region Egg Type Assignment

        public virtual void AssignEggsType()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            int bombCount = 0;
            // Only call if player is master client.
            PlayerManager.Instance.FindPlayersInRooms();
            for (int i = 0; i < PlayerManager.Instance.PlayerInRoom.Count; i++)
            {
                if (PlayerManager.Instance.PlayerInRoom.Count - i == 1 && bombCount == 0)
                {
                    PlayerManager.Instance.PlayerInRoom[i].CharacterEggManager.Egg.RPC_ChangeType(EggType.Bomb);
                    bombCount++;
                }
                else
                {
                    EggType type = Randomizer.RandomBetween(0, 1) == 0 ? EggType.Normal : EggType.Bomb;
                    PlayerManager.Instance.PlayerInRoom[i].CharacterEggManager.Egg.RPC_ChangeType(type);
                    if (type == EggType.Bomb)
                        bombCount++;

                }
            }
        }

        [PunRPC]
        public virtual void ChangeEggType(EggType newType)
        {
            PlayerManager.Instance.LocalPlayer.CharacterEggManager.Egg.RPC_ChangeType(newType);
        }

        #endregion


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

            }
        }

        public virtual void ResetDeathCount()
        {
            for (int i = 0; i < _playerMatchDataList.Count; i++)
            {
                _playerMatchDataList[i].EggDestroyed = false;
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

        public List<PlayerMatchData> PlayerMatchDataList => _playerMatchDataList;

    }

    public enum Turn
    {
        Self,
        Enemy,
        Teammate
    }
}

