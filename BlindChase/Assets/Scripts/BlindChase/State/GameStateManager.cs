using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase.State 
{
    public class GameEffect
    { 
        public Action EffectCall { get; private set; }
        public int Delay { get; set; }

        public GameEffect(Action action, int delay = 0) 
        {
            EffectCall = action;
            Delay = delay;
        }
    }

    // Responsible for handling state changes trihhered from moment-to-moment gameplay.
    public class GameStateManager
    {
        GameState m_current;
        GameState m_previous;



        // Add all game states into the dictionary
        Dictionary<Type, GameState> m_gameStateCollection = new Dictionary<Type, GameState>
            {
                { typeof(TurnStart), new TurnStart() },
                { typeof(TurnEnd), new TurnEnd() },
                { typeof(PlayerTurn), new PlayerTurn() }
            };

        public void InitGame(OptionManager optionManager) 
        {
        
        }


        public void Init(Stack<GameEffect> startEffects)
        {
            m_current = m_gameStateCollection[typeof(TurnStart)];
            m_current.Init(startEffects);
            OnEnterState();
        }

        void OnEnterState()
        {
            m_current?.OnEnter(m_previous);
        }

        public void TransitionToNextState(Stack<GameEffect> startEffects)
        {
            m_previous = m_current;
            m_current = m_gameStateCollection[m_current.TransitionTo()];
            m_current.Init(startEffects);
            OnEnterState();
        }

        

    }


}
