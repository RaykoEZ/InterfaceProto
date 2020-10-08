using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase.State
{
    public abstract class TurnState
    {
        public event OnTurnStateTransition OnGameStateTransition;
        protected Type NextState = default;

        public virtual void Init() 
        {
        }

        // Type returned = the next game state.
        public virtual void OnEnter(TurnState incomingState) 
        {
        }

        public virtual Type TransitionTo() 
        {
            OnGameStateTransition?.Invoke(NextState);
            return NextState;
        }

    }

}