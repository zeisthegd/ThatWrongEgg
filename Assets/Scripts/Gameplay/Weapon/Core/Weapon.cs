using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;
using Penwyn.Tools;

using Photon.Pun;

namespace Penwyn.Game
{
    public class Weapon : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        [Header("Data")]
        [Expandable]
        public WeaponData CurrentData;
        [HorizontalLine]

        [Header("Graphics")]
        public Animator Animator;
        public SpriteRenderer SpriteRenderer;
        [HorizontalLine]

        [Header("Input")]
        public WeaponInputType InputType;

        [Header("Feedbacks")]
        public Feedbacks UseFeedbacks;

        [Header("Owner")]
        [ReadOnly] public Character Owner;
        [ReadOnly] public Energy Energy;

        [ReadOnly][SerializeField] protected WeaponState _currentWeaponState;
        protected WeaponAim _weaponAim;
        protected WeaponAutoAim _weaponAutoAim;
        protected Coroutine _cooldownCoroutine;

        public event UnityAction RequestUpgradeEvent;
        public event UnityAction WeaponUsed;

        protected virtual void Awake()
        {
            GetComponents();
        }

        public virtual void Initialization()
        {
        }

        protected virtual void Update()
        {
            UpdateInputEvents();
        }

        public virtual void RequestWeaponUse()
        {
            //*Derive this
            if (_currentWeaponState == WeaponState.WeaponIdle)
            {
                UseWeapon();
            }
        }

        protected virtual void UseWeapon()
        {
            _currentWeaponState = WeaponState.WeaponUse;
            if (UseFeedbacks != null)
                UseFeedbacks.PlayFeedbacks();
        }

        public virtual void InvokeWeaponUsedEvent()
        {
            WeaponUsed?.Invoke();
        }

        public virtual void UseWeaponTillNoTargetOrEnergy()
        {
            if (_weaponAutoAim)
            {
                _weaponAutoAim.FindTarget();
            }
            RequestWeaponUse();
        }
        public virtual void StartCooldown()
        {
            _currentWeaponState = WeaponState.WeaponCooldown;
            _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        }

        protected virtual IEnumerator CooldownCoroutine()
        {
            yield return new WaitForSeconds(CurrentData.Cooldown);
            _currentWeaponState = WeaponState.WeaponIdle;
        }

        /// <summary>
        /// Load the weapon data from a scriptable data.
        /// </summary>
        public virtual void LoadWeapon(WeaponData data)
        {
            CurrentData = data;
            //SpriteRenderer.sprite = data.Icon;
        }

        [Button("Load Weapon Data")]
        public virtual void LoadWeapon()
        {
            if (CurrentData != null)
                LoadWeapon(CurrentData);
            else
                Debug.Log("Please insert Weapon Data");
        }


        #region Upgrade

        [Button("Reques Upgrade", EButtonEnableMode.Playmode)]
        public virtual void RequestUpgrade()
        {
            RequestUpgradeEvent?.Invoke();
        }

        [Button("Upgrade (Random Data)")]
        public virtual void RandomUpgrade()
        {
            Upgrade(CurrentData.Upgrades[Randomizer.RandomNumber(0, CurrentData.Upgrades.Count)]);
        }

        public virtual void Upgrade(WeaponData data)
        {
            if (CurrentData != null)
            {
                if (CurrentData.Upgrades != null)
                {
                    LoadWeapon(data);
                    Owner.Health.Set(Owner.Health.CurrentHealth, CurrentData.RequiredUpgradeValue);
                }
                else
                {
                    Debug.Log("Max level reached!");
                }
            }
            else
                Debug.Log("Please insert Weapon Data!");
        }

        public virtual void CheckUpgradeRequirements()
        {
            if (CurrentData.AutoUpgrade)
            {
                if (Owner.Health.CurrentHealth == CurrentData.RequiredUpgradeValue)
                    RequestUpgrade();
            }
        }

        #endregion

        #region  Input Events
        public virtual void UpdateInputEvents()
        {
            if (InputType == WeaponInputType.NormalAttack && InputReader.Instance.IsHoldingNormalAttack)
            {
                UseWeaponTillNoTargetOrEnergy();
            }
        }
        #endregion


        public virtual void GetComponents()
        {
            _weaponAim = GetComponent<WeaponAim>();
            _weaponAutoAim = GetComponent<WeaponAutoAim>();
            Energy = GetComponent<Energy>();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] data = info.photonView.InstantiationData;
            this.gameObject.name = (string)data[0];
            this.Owner = PlayerManager.Instance.FindByOwnerActorNumber((int)data[1]);
            Debug.Log(Owner == null);
            this.Owner.CharacterWeaponHandler.SetWeapon(this);
        }


        public virtual void OnEnable()
        {
            _currentWeaponState = WeaponState.WeaponIdle;
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            StopAllCoroutines();
        }


    }
}
