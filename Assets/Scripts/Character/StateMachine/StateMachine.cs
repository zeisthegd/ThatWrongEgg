using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penwyn.Game
{
    public class StateMachine<T>
    {
        public T CurrentState { get; protected set; }
        public T PreviousState { get; protected set; }

        public StateMachine(T initState)
        {
            CurrentState = initState;
        }

        public virtual void ChangeState(T newState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
        }
    }
}

