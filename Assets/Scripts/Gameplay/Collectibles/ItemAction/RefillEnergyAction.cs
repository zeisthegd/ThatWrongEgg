using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Penwyn.Tools;
namespace Penwyn.Game
{
    public class RefillEnergyAction : ItemAction
    {
        public float Amount;

        public override void Act()
        {
            base.Act();
        }

        public override void Act(GameObject gameObject)
        {
            base.Act(gameObject);
            Character character = gameObject.FindComponent<Character>();
            if (character != null)
            {
                character.CharacterWeaponHandler.CurrentWeapon.Energy.Add(Amount);
            }
        }
    }
}

