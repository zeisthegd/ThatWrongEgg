using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
using TMPro;

using Penwyn.Game;
using Penwyn.Tools;

namespace Penwyn.UI
{
    public class HUD : SingletonMonoBehaviour<HUD>
    {
        [Header("Player")]
        public Button ItemButton;
        [Header("Match")]
        public TMP_Text TimerTxt;


        protected Character _localPlayer;
        protected Weapon _localPlayerWeapon;

        protected virtual void Awake()
        {
            PlayerManager.Instance.PlayerSpawned += OnPlayerSpawned;
        }


        public virtual void Update()
        {
            TimerTxt.SetText(CombatManager.Instance.CurrentRoundTime + "");
        }


        #region  Weapon HUD

        public virtual void SetWeaponButtonIcon()
        {

        }

        protected virtual void OnItemChanged()
        {
            if (_localPlayer.CharacterWeaponHandler != null)
            {
                _localPlayerWeapon = _localPlayer.CharacterWeaponHandler.CurrentWeapon;
                SetWeaponButtonIcon();
            }
        }

        #endregion

        protected virtual void OnPlayerSpawned()
        {
            _localPlayer = PlayerManager.Instance.LocalPlayer;
            OnItemChanged();
            ConnectEvents();
        }



        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {
            DisconnectEvents();
        }

        public virtual void ConnectEvents()
        {
            if (_localPlayer.CharacterWeaponHandler)
                _localPlayer.CharacterWeaponHandler.WeaponChanged += OnItemChanged;
        }

        public virtual void DisconnectEvents()
        {
            if (_localPlayer.CharacterWeaponHandler)
                _localPlayer.CharacterWeaponHandler.WeaponChanged -= OnItemChanged;
            PlayerManager.Instance.PlayerSpawned -= OnPlayerSpawned;
        }
    }
}

