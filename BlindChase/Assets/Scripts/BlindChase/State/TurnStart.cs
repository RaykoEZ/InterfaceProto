using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    public class TurnStart : TurnState
    {
        public override void Init()
        {
            base.Init();
            NextState = typeof(ActionPhase);
        }

        public override void OnEnter(TurnState incomingState)
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