using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using Penwyn.Game;

namespace Penwyn.UI
{
    public class RoomUI : MonoBehaviour
    {
        [SerializeField] TMP_Text passcodeTxt;
        [SerializeField] Button openSettingsBtn;
        [SerializeField] Button startMatchBtn;

        [Header("Teams")]
        public List<TMP_Text> PlayerNames;
        [Header("Score")]
        public List<TMP_Text> PlayerScores;

        [Header("Turns")]
        public TMP_Text TurnTextPrefab;
        public TMP_Text CurrentTurn;
        public Transform Container;

        void Awake()
        {
            PhotonTeamsManager.PlayerJoinedTeam += ShowTeams;
            CombatManager.Instance.ScoreChanged += ShowScore;
        }

        void Start()
        {
            if (PhotonNetwork.InRoom)
                passcodeTxt.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["Passcode"];

            if (!PhotonNetwork.IsMasterClient)
            {
                openSettingsBtn.gameObject.SetActive(false);
                startMatchBtn.gameObject.SetActive(false);
            }
        }

        public void StartMatch()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                GameManager.Instance.RPC_StartGame();
                openSettingsBtn.gameObject.SetActive(false);
                startMatchBtn.gameObject.SetActive(false);
            }
        }

        public virtual void OnGameStarted()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void ShowTeams(Photon.Realtime.Player player, PhotonTeam teamJoined)
        {
            for (int i = 0; i < CombatManager.Instance.Teams.Count; i++)
            {
                if (CombatManager.Instance.Teams[i].Players != null && CombatManager.Instance.Teams[i].Players.Length > 0)
                {
                    Debug.Log($"{i}|{CombatManager.Instance.Teams[i].Team.Name}|{CombatManager.Instance.Teams[i].Players[0].NickName}");
                    PlayerNames[i].SetText(CombatManager.Instance.Teams[i].Players[0].NickName + "");
                }
            }
            Debug.Log("Showed teams!");
        }

        public virtual void ShowScore()
        {
            for (int i = 0; i < CombatManager.Instance.Teams.Count; i++)
            {
                if (CombatManager.Instance.Teams[i].Players != null)
                    PlayerScores[i].SetText(CombatManager.Instance.Teams[i].Score + "");
            }
            Debug.Log("Showed scores!");
        }


        void OnEnable()
        {
            // GameManager.Instance.GameStarted += OnGameStarted;
        }

        void OnDisable()
        {
            //  GameManager.Instance.GameStarted -= OnGameStarted;
        }
    }

}

