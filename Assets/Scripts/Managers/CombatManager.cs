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
        public string GolfBallPath;

        protected PhotonView _photonView;


        protected List<PlayerMatchData> _playerMatchDataList = new List<PlayerMatchData>();

        public event UnityAction TurnChanged;
        public event UnityAction ScoreChanged;

        protected MatchSettings _matchSettings;
        protected GolfBall Ball;

        protected float _timerStart = 0;

        public virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            PlayerMatchDatas();
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

        #region Ball Spqwning -------------------------------------------------------------------------------------

        public virtual void CreateBallOnMaster()
        {
            // Only act if player is master client.
            if (PhotonNetwork.IsMasterClient == true)
            {
                Ball = PhotonNetwork.Instantiate(GolfBallPath, Vector3.up, Quaternion.identity).GetComponent<GolfBall>();
                Ball.PlayerTouched += RPC_GolfTouchPlayer;
            }
        }

        public virtual void AssignBall(GolfBall ball)
        {
            if (Ball == null)
                Ball = ball;
            DontDestroyOnLoad(Ball);
        }

        #endregion

        #region Golf Touch Player -------------------------------------------------------------------------------------

        public virtual void RPC_GolfTouchPlayer(Character character)
        {
            _photonView.RPC(nameof(GolfTouchPlayer), RpcTarget.All, character.photonView.OwnerActorNr);
        }

        [PunRPC]
        public virtual void GolfTouchPlayer(int playerOwnerActorNumber)
        {
            Character player = PlayerManager.Instance.FindByOwnerActorNumber(playerOwnerActorNumber);
            Debug.Log(player);
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
        }

        public virtual void DisconnectEvents()
        {
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
}

