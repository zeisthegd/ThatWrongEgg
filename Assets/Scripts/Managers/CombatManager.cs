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
using System;

namespace Penwyn.Game
{
    public class CombatManager : SingletonMonoBehaviour<CombatManager>
    {
        [ReadOnly] public Player CurrentPlayer;
        protected PhotonView _photonView;


        protected List<PlayerMatchData> _playerMatchDataList = new List<PlayerMatchData>();

        public event UnityAction TurnChanged;
        public event UnityAction ScoreChanged;

        protected MatchSettings _matchSettings;


        protected float _timerStart = 0;

        public virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            PlayerMatchDatas();
        }

        protected virtual void Update()
        {
            HandleRoundTimeEnded();
        }

        public virtual void StartGame()
        {
            ConnectPlayerEvents();
            _matchSettings = GameManager.Instance.MatchSettings;
        }

        public virtual void PlayerMatchDatas()
        {
            _playerMatchDataList = new List<PlayerMatchData>();
            for (int i = 0; i < 4; i++)
            {
                _playerMatchDataList.Add(new PlayerMatchData());
            }
        }

        #region Egg Type Assignment-------------------------------------------------------------------------------------

        public virtual void AssignEggsType()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            // Only call if player is master client.
            int bombCount = 0;
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

        #endregion

        #region Timer and Time Handler-------------------------------------------------------------------------------------

        public virtual void MasterRPC_StartTimer()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _photonView.RPC(nameof(StartTimer), RpcTarget.AllViaServer, PhotonNetwork.ServerTimestamp / 1000);
            }
        }

        [PunRPC]
        public virtual void StartTimer(int serverTime)
        {
            _timerStart = serverTime;
        }

        private void HandleRoundTimeEnded()
        {
            if (CurrentRoundTime == 0 && GameManager.Instance.IsGameStarted)
            {
                //Disable input.
                InputReader.Instance.DisableGameplayInput();
                //Explode all bomb eggs.
                var eggs = FindObjectsOfType<Egg>();
                for (int i = 0; i < eggs.Length; i++)
                {
                    if (eggs[i].Type == EggType.Bomb)
                    {
                        eggs[i].Explode();
                    }
                }
                GameManager.Instance.RPC_LoadNextLevel();
            }
        }

        #endregion

        #region Egg Death-------------------------------------------------------------------------------------

        public virtual void LocalPlayerEggDeath()
        {
            _photonView.RPC(nameof(PlayerEggDeath), RpcTarget.All, PhotonNetwork.LocalPlayer);
        }

        [PunRPC]
        public virtual void PlayerEggDeath(Player player)
        {
            Debug.Log(player.NickName);
            ScoreChanged?.Invoke();
        }

        public virtual void ResetDeathCount()
        {
            for (int i = 0; i < _playerMatchDataList.Count; i++)
            {
                _playerMatchDataList[i].EggDestroyed = false;
            }
        }

        #endregion

        public virtual void ConnectPlayerEvents()
        {
            PlayerManager.Instance.LocalPlayer.CharacterEggManager.Egg.Health.OnDeath += LocalPlayerEggDeath;
        }

        public virtual void DisconnectEvents()
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null)
            {
                PlayerManager.Instance.LocalPlayer.CharacterEggManager.Egg.Health.OnDeath -= LocalPlayerEggDeath;
            }
        }

        protected virtual void OnDisable()
        {
            DisconnectEvents();
        }

        public List<PlayerMatchData> PlayerMatchDataList => _playerMatchDataList;
        /// <summary>
        /// Round time - Elapsed time.
        /// </summary>
        public int CurrentRoundTime
        {
            get
            {
                if (GameManager.Instance.IsGameStarted)
                    return (int)(_matchSettings.RoundTime - (PhotonNetwork.ServerTimestamp / 1000 - _timerStart));
                else
                    return 0;
            }
        }
    }

    public enum Turn
    {
        Self,
        Enemy,
        Teammate
    }
}

