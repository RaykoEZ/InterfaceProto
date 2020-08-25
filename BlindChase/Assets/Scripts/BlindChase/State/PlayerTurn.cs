using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    // Need to listen to win detector, if won/loss, => GaemEnd
    public class PlayerTurn : GameState
    {
        public override void Init(List<GameEffect> startEffects)
        {
            base.Init(startEffects);
            NextState = typeof(TurnEnd);

        }

        public override void OnEnter(GameState incomingState)
        {
            base.OnEnter(incomingState);

        }
        public override Type TransitionTo() 
        {
            return base.TransitionTo();
        }
    }

}