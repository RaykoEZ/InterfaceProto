using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase.State
{
    public abstract class GameState
    {
        public event OnGameStateTransition OnGameStateTransition;
        protected Stack<DelayedEffect> m_stateEffects = new Stack<DelayedEffect>();
        protected Type NextState = typeof(GameState);

        public virtual void Init(List<DelayedEffect> startEffects) 
        {
            if(startEffects == null) 
            {
                return;
            }

            AddEffects(startEffects);
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

        public void AddEffects(List<DelayedEffect> gameEffect) 
        {
            foreach (DelayedEffect effect in gameEffect)
            {
                m_stateEffects.Push(effect);
            }
        }

        public virtual Type TransitionTo() 
        {
            OnGameStateTransition?.Invoke(NextState);
            return NextState;
        }

        protected Stack<DelayedEffect> ProcessEffectStack()
        {
            Stack<DelayedEffect> delayedEffects = new Stack<DelayedEffect>();

            if (m_stateEffects == null || m_stateEffects.Count == 0)
            {
                return delayedEffects;
            }

            Type gameStateType = GetType();
            // Call all effects in this effect stack.
            for (int i = 0; i < m_stateEffects.Count; ++i)
            {
                DelayedEffect effect = m_stateEffects.Peek();

                effect.Delay--;

                if (effect.Delay < 0)
                {
                    // This may need to be a coroutine
                    effect.Effect.Activate(effect.Args);
                    m_stateEffects.Pop();
                }
                else
                {
                    delayedEffects.Push(m_stateEffects.Pop());
                }
            }

            return delayedEffects;
        }

    }

}