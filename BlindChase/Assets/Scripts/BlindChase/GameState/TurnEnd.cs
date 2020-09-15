using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    public class TurnEnd : GameState
    {
        public override void Init(List<DelayedEffect> startEffects)
        {
            base.Init(startEffects);
            NextState = typeof(TurnStart);
        }

        public override void OnEnter(GameState incomingState)
        {
            base.OnEnter(incomingState);
            TransitionTo();
        }
        public override Type TransitionTo() 
        {          
            return base.TransitionTo(); 
        }
    }

}