using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase.State
{
    public abstract class GameState
    {
        protected Stack<GameEffect> m_stateEffects = new Stack<GameEffect>();
        public event OnGameStateTransition OnGameStateTransition;
        protected Type NextState = typeof(GameState);

        public virtual void Init(List<GameEffect> startEffects) 
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

        public void AddEffects(List<GameEffect> gameEffect) 
        {
            foreach (GameEffect effect in gameEffect)
            {
                m_stateEffects.Push(effect);
            }
        }

        public virtual Type TransitionTo() 
        {
            OnGameStateTransition?.Invoke(NextState);
            return NextState;
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

}