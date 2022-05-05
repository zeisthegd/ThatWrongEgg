using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using Photon;
using Photon.Pun;
namespace Penwyn.Game
{
    public class CharacterWeaponHandler : CharacterAbility
    {
        [Header("Weapon Data")]
        public Weapon InitialWeaponPrefab;
        public WeaponData InitialWeaponData;
        public string InitialWeaponPrefabPath;
        [HorizontalLine]

        [Header("Weapon Holder")]
        public Transform WeaponHolder;

        protected Weapon _currentWeapon;
        protected WeaponData _currentWeaponData;
        public event UnityAction WeaponChanged;

        public override void AwakeAbility(Character character)
        {
            base.AwakeAbility(character);
            ChangeWeapon(InitialWeaponData);
        }
        public virtual void CreateWeapon()
        {
            if (WeaponHolder == null)
                WeaponHolder = this.transform;
            if (_character.photonView.IsMine)
            {
                object[] data = new object[] { WeaponObjectName(), _character.photonView.OwnerActorNr };
                _currentWeapon = PhotonNetwork.Instantiate(InitialWeaponPrefabPath, WeaponHolder.position, Quaternion.identity, 0, data).GetComponent<Weapon>();
                _currentWeapon.Owner = this._character;
                _currentWeapon.Initialization();
                _currentWeapon.LoadWeapon(InitialWeaponData);
                _currentWeapon.transform.SetParent(WeaponHolder);
            }
        }

        public virtual void ChangeWeapon(WeaponData newData)
        {
            if (_currentWeapon == null)
                CreateWeapon();
            else
            {
                _currentWeapon.LoadWeapon(newData);
            }
            WeaponChanged?.Invoke();
        }

        public virtual void SetWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            _currentWeapon.Owner = this._character;
            _currentWeapon.Initialization();
            _currentWeapon.LoadWeapon(InitialWeaponData);
            _currentWeapon.transform.SetParent(WeaponHolder);
        }

        public virtual string WeaponObjectName()
        {
            return $"{_character.photonView.OwnerActorNr}_Weapon";
        }



        public Weapon CurrentWeapon { get => _currentWeapon; }
        public WeaponData WeaponData { get => _currentWeaponData; }
    }
}