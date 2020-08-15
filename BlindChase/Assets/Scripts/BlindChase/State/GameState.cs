using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.State
{

    public abstract class GameState
    {
        protected Stack<GameEffect> m_stateEffects;

        public virtual void Init(Stack<GameEffect> startEffects) 
        {
            m_stateEffects = startEffects;
        }

        // Type returned = the next game state.
        public virtual void OnEnter(GameState incomingState) 
        {
            // Empty all incoming effects at the start of current game state
            if (m_stateEffects != null && m_stateEffects.Count > 0) 
            {
                m_stateEffects = ProcessEffectStack();
            }
        }

        public virtual Type TransitionTo() 
        {
            return GetType();
        }

        protected Stack<GameEffect> ProcessEffectStack()
        {
            Stack<GameEffect> delayedEffects = new Stack<GameEffect>();

            if (m_stateEffects == null || m_stateEffects.Count == 0)
            {
                return delayedEffects;
            }

            Type gameStateType = GetType();
            // Call all effects in this effect stack.
            for (int i = 0; i < m_stateEffects.Count; ++i)
            {
                GameEffect effect = m_stateEffects.Peek();

                effect.Delay--;

                if (effect.Delay < 0)
                {
                    // This may need to be a coroutine
                    m_stateEffects.Pop().EffectCall.Invoke();
                }
                else
                {
                    delayedEffects.Push(m_stateEffects.Pop());
                }
            }

            return delayedEffects;
        }

    }

    public class TurnStart : GameState
    {
        public override void Init(Stack<GameEffect> startEffects)
        {
            base.Init(startEffects);

        }

        public override void OnEnter(GameState incomingState)
        {
            base.OnEnter(incomingState);

        }
        public override Type TransitionTo()
        {
            base.TransitionTo();
            return typeof(PlayerTurn);
        }
    }

    // Need to listen to win detector, if won/loss, => GaemEnd
    public class PlayerTurn : GameState
    {
        public override void Init(Stack<GameEffect> startEffects)
        {
            base.Init(startEffects);

        }

        public override void OnEnter(GameState incomingState)
        {
            base.OnEnter(incomingState);

        }
        public override Type TransitionTo() 
        {
            base.TransitionTo();
            return typeof(TurnEnd); 
        }
    }


    public class TurnEnd : GameState
    {
        public override void Init(Stack<GameEffect> startEffects)
        {
            base.Init(startEffects);

        }

        public override void OnEnter(GameState incomingState)
        {
            base.OnEnter(incomingState);

        }
        public override Type TransitionTo() 
        {
            base.TransitionTo();
            return typeof(TurnStart); 
        }
    }

    public class GameEnd : GameState
    {
        public override void Init(Stack<GameEffect> startEffects)
        {
            base.Init(startEffects);

        }

        public override void OnEnter(GameState incomingState)
        {
            base.OnEnter(incomingState);

        }
        public override Type TransitionTo()
        {
            base.TransitionTo();
            return null;
        }
    }

}