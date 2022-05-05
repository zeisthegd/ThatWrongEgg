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
        public ProgressBar PlayerEnergy;
        public TMP_Text PlayerMoney;
        public Button WeaponButton;
        public List<WeaponUpgradeButton> WeaponUpgradeButtons;


        protected Character _localPlayer;
        protected Weapon _localPlayerWeapon;
        protected virtual void Awake()
        {
            PlayerManager.Instance.PlayerSpawned += OnPlayerSpawned;
        }

        protected virtual void Update()
        {
            UpdateLevelProgress();
        }

        #region Level HUD

        public virtual void UpdateLevelProgress()
        {

        }

        #endregion

        #region Player Stats HUD


        protected virtual void UpdateMoney()
        {
            if (PlayerMoney != null)
                PlayerMoney.SetText(_localPlayer.CharacterMoney.CurrentMoney + "");
        }

        #region  Weapon HUD

        public virtual void SetEnergyBar()
        {
            if (PlayerEnergy != null)
            {
                PlayerEnergy.SetMaxValue(_localPlayer.Energy.MaxEnergy);
                PlayerEnergy.SetValue(_localPlayer.Energy.CurrentEnergy);
            }
        }

        protected virtual void UpdateEnergy()
        {
            PlayerEnergy.SetValue(_localPlayer.Energy.CurrentEnergy);
            if (_localPlayer.Energy.MaxEnergy != PlayerEnergy.ActualValue.maxValue)
            {
                PlayerEnergy.SetMaxValue(_localPlayer.Energy.MaxEnergy);
                Debug.Log(PlayerEnergy.ActualValue.maxValue);
            }
        }

        public virtual void SetWeaponButtonIcon()
        {
            if (WeaponButton != null)
                WeaponButton.image.sprite = _localPlayerWeapon.CurrentData.Icon;
        }

        #endregion

        #region Weapon Upgrades

        public virtual void LoadAvailableUpgrades()
        {
            if (_localPlayer.CharacterWeaponHandler.CurrentWeapon.CurrentData.Upgrades.Count <= 0)
                return;
            Time.timeScale = 0;
            for (int i = 0; i < _localPlayer.CharacterWeaponHandler.CurrentWeapon.CurrentData.Upgrades.Count; i++)
            {
                WeaponUpgradeButtons[i].Set(_localPlayer.CharacterWeaponHandler.CurrentWeapon.CurrentData.Upgrades[i]);
                WeaponUpgradeButtons[i].gameObject.SetActive(true);
            }
        }

        public virtual void EndWeaponUpgrades()
        {
            SetWeaponButtonIcon();
            Time.timeScale = 1;
            for (int i = 0; i < WeaponUpgradeButtons.Count; i++)
            {
                WeaponUpgradeButtons[i].gameObject.SetActive(false);
            }
        }

        public virtual void ConnectEndWeaponUpgradesEvents()
        {
            for (int i = 0; i < WeaponUpgradeButtons.Count; i++)
            {
                WeaponUpgradeButtons[i].DataChosen += EndWeaponUpgrades;
            }
        }

        public virtual void DisconnectEndWeaponUpgradesEvents()
        {
            for (int i = 0; i < WeaponUpgradeButtons.Count; i++)
            {
                WeaponUpgradeButtons[i].DataChosen -= EndWeaponUpgrades;
            }
        }


        #endregion

        protected virtual void OnPlayerSpawned()
        {
            _localPlayer = PlayerManager.Instance.LocalPlayer;
            OnWeaponChanged();
            ConnectEvents();
        }

        protected virtual void OnWeaponChanged()
        {
            _localPlayerWeapon = _localPlayer.CharacterWeaponHandler.CurrentWeapon;
            SetEnergyBar();
            SetWeaponButtonIcon();
        }

        #endregion

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {
            DisconnectEvents();
        }

        public virtual void ConnectEvents()
        {
            _localPlayer.CharacterWeaponHandler.WeaponChanged += OnWeaponChanged;
            _localPlayer.Energy.OnChanged += UpdateEnergy;
            // _localPlayer.CharacterMoney.MoneyChanged += UpdateMoney;
            _localPlayerWeapon.RequestUpgradeEvent += LoadAvailableUpgrades;
            ConnectEndWeaponUpgradesEvents();
        }

        public virtual void DisconnectEvents()
        {
            _localPlayer.Energy.OnChanged -= UpdateEnergy;
            _localPlayer.CharacterWeaponHandler.WeaponChanged -= OnWeaponChanged;
            //_localPlayer.CharacterMoney.MoneyChanged -= UpdateMoney;
            _localPlayerWeapon.RequestUpgradeEvent -= LoadAvailableUpgrades;
            PlayerManager.Instance.PlayerSpawned -= OnPlayerSpawned;
            DisconnectEndWeaponUpgradesEvents();
        }
    }
}

