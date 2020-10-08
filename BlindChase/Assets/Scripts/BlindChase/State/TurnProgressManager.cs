using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase.State 
{
    // Responsible for handling state changes triggered from moment-to-moment gameplay.
    public class TurnProgressManager
    {
        TurnState m_current;
        TurnState m_previous;

        protected event OnTurnStateTransition OnGameStateUpdate = default;
        public event OnStateChange OnTurnStart = default;
        public event OnStateChange OnTurnEnd = default;

        // Add all game states into the dictionary
        Dictionary<Type, TurnState> m_turnStateCollection = new Dictionary<Type, TurnState>
        {
            { typeof(TurnStart), new TurnStart() },
            { typeof(TurnEnd), new TurnEnd() },
            { typeof(ActionPhase), new ActionPhase() }
        };

        public void StartGame()
        {
            SetCurrentState(typeof(TurnStart));
        }

        public void Shutdown() 
        {
            OnTurnStart = null;
            OnTurnEnd = null;
        }

        public Type CurrentGameState() 
        {
            return m_current.GetType();
        }

        void SetCurrentState(Type type) 
        {
            m_current = m_turnStateCollection[type];
            m_current.Init();
            m_current.OnGameStateTransition += OnStateTransition;
            m_current?.OnEnter(m_previous);
        }

        void OnStateTransition(Type type) 
        {
            if (type == typeof(ActionPhase))
            {
                OnTurnStart?.Invoke();
            }
            if (type == typeof(TurnEnd))
            {
                OnTurnEnd?.Invoke();
            }
            TransitionToNextState(type);
            OnGameStateUpdate?.Invoke(type);
        }

        public void TransitionToNextState() 
        {
            m_current.TransitionTo();
        }

        void TransitionToNextState(Type newGameState)
        {
            m_current.OnGameStateTransition -= OnStateTransition;
            m_previous = m_current;
            SetCurrentState(newGameState);
        }

    }


}
