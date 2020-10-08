using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    public class TurnEnd : TurnState
    {
        public override void Init()
        {
            base.Init();
            NextState = typeof(TurnStart);
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