﻿using System;
using System.Collections.Generic;

namespace BlindChase.State
{
    public class TurnStart : GameState
    {
        public override void Init(List<GameEffect> startEffects)
        {
            base.Init(startEffects);
            NextState = typeof(PlayerTurn);
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