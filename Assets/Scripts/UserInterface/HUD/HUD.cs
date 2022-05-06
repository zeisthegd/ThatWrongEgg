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
        public Button WeaponButton;


        protected Character _localPlayer;
        protected Weapon _localPlayerWeapon;
        protected virtual void Awake()
        {
            PlayerManager.Instance.PlayerSpawned += OnPlayerSpawned;
        }


        #region  Weapon HUD
        
        public virtual void SetWeaponButtonIcon()
        {
            if (WeaponButton != null)
                WeaponButton.image.sprite = _localPlayerWeapon.CurrentData.Icon;
        }

         protected virtual void OnWeaponChanged()
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
            OnWeaponChanged();
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
                _localPlayer.CharacterWeaponHandler.WeaponChanged += OnWeaponChanged;
        }

        public virtual void DisconnectEvents()
        {
            if (_localPlayer.CharacterWeaponHandler)
                _localPlayer.CharacterWeaponHandler.WeaponChanged -= OnWeaponChanged;
            PlayerManager.Instance.PlayerSpawned -= OnPlayerSpawned;
        }
    }
}

