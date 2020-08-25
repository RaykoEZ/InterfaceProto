using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    public class GameEnd : GameState
    {
        public override void Init(List<GameEffect> startEffects)
        {
            base.Init(startEffects);
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