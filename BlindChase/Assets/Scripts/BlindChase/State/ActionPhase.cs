using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    // Need to listen to win detector, if won/loss, => GaemEnd
    public class ActionPhase : TurnState
    {
        public override void Init()
        {
            base.Init();
            NextState = typeof(TurnEnd);
        }

        public override void OnEnter(TurnState incomingState)
        {
            base.OnEnter(incomingState);

        }
        public override Type TransitionTo() 
        {
            return base.TransitionTo();
        }
    }

}