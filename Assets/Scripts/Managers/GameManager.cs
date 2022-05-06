using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Photon;
using Photon.Pun;
using Photon.Realtime;

using NaughtyAttributes;

using Penwyn.Tools;
using Penwyn.UI;

namespace Penwyn.Game
{

    public class GameManager : MonoBehaviourPun
    {
        [Header("Managers")]
        [Expandable] public NetworkManager NetworkManager;
        [Expandable] public CameraManager CameraManager;
        [Expandable] public SceneManager SceneManager;
        [Expandable] public PlayerManager PlayerManager;
        [Expandable] public InputManager InputManager;

        public AudioPlayer AudioPlayer;
        public CombatManager CombatManager;
        public LevelManager LevelManager;

        [Expandable] public MatchSettings MatchSettings;

        public static GameManager Instance;
        public event UnityAction GameStarted;
        protected GameState _gameState;


        void Awake()
        {
            CheckSingleton();
        }

        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;
        }

        public virtual void RPC_StartGame()
        {
            photonView.RPC(nameof(StartGame), RpcTarget.All);
        }

        public virtual void RPC_LoadNextLevel()
        {
            photonView.RPC(nameof(LoadNextLevel), RpcTarget.All);
        }

        [PunRPC]
        public virtual void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
            GameStarted?.Invoke();
        }

        [PunRPC]
        public virtual void LoadNextLevel()
        {
            StartCoroutine(LoadNextLevelCoroutine());
        }

        /// <summary>
        /// Disable input and load the first level.
        /// Create turns queue.
        /// Load the first turn;
        /// </summary>
        public IEnumerator StartGameCoroutine()
        {
            _gameState = GameState.LevelLoading;

            InputReader.Instance.DisableGameplayInput();
            CombatManager.StartGame();
            LevelManager.LoadLevelByIndex(0);
            PlayerManager.Instance.FindPlayersInRooms();

            yield return new WaitForSeconds(MatchSettings.LevelLoadTime + MatchSettings.PlayerPositioningTime);

            InputReader.Instance.EnableGameplayInput();
            CombatManager.AssignEggsType();
            CombatManager.MasterRPC_StartTimer();

            _gameState = GameState.Started;
        }

        /// <summary>
        /// Disable input and load the next level.
        /// Load the first turn;
        /// </summary>
        public IEnumerator LoadNextLevelCoroutine()
        {
            _gameState = GameState.LevelLoading;

            yield return new WaitForSeconds(MatchSettings.LevelLoadTime + MatchSettings.PlayerPositioningTime);

            InputReader.Instance.DisableGameplayInput();
            PlayerManager.LocalPlayer.Health.Reset();//TODO change to revive eggs.
            LevelManager.LoadNextLevel();

            yield return new WaitForSeconds(MatchSettings.LevelLoadTime + MatchSettings.PlayerPositioningTime);

            CombatManager.ResetDeathCount();
            InputReader.Instance.EnableGameplayInput();
            CombatManager.AssignEggsType();
            CombatManager.MasterRPC_StartTimer();

            _gameState = GameState.Started;
        }

        void CheckSingleton()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }


        public virtual void OnRoomSceneLoaded()
        {
            PlayerManager.CreateLocalPlayer();
            _gameState = GameState.TeamChoosing;
        }

        void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == SceneManager.RoomSceenName)
            {
                OnRoomSceneLoaded();
            }
        }

        public GameState State => _gameState;
        public bool IsGameStarted => State == GameState.Started;

    }

    public enum GameState
    {
        NotInRoom,
        TeamChoosing,
        LevelLoading,
        Started
    }
}
