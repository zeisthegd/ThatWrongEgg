using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

namespace Penwyn.Game
{
    public class Energy : MonoBehaviour
    {
        public float MaxEnergy = 10;

        public event UnityAction OnChanged;
        public event UnityAction OutOfEnergy;

        [SerializeField][ReadOnly] protected float _energy = 0;
        protected Character _character;

        void Start()
        {
            _character = GetComponent<Character>();
            _energy = MaxEnergy;
        }

        public virtual void Use(float value, bool allowUseWhenAboveZero = false)
        {
            if (_energy - value >= 0 || (_energy > 0 && allowUseWhenAboveZero))
            {
                _energy -= value;
                OnChanged?.Invoke();
            }
            if (_energy <= 0)
                OutOfEnergy?.Invoke();
        }

        public virtual void Add(float addValue)
        {
            Set(_energy + addValue, MaxEnergy);
        }

        public virtual void Set(float newValue)
        {
            Set(newValue, newValue);
        }

        public virtual void Set(float newValue, float maxEnergy)
        {
            MaxEnergy = maxEnergy;
            _energy = Mathf.Clamp(newValue, 0, MaxEnergy);
            OnChanged?.Invoke();
        }

        public float CurrentEnergy { get => _energy; }
    }
}

