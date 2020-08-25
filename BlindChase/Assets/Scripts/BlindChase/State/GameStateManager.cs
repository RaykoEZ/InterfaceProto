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

    public delegate void OnGameStateTransition(Type gameStateType);
    public delegate void OnTurnChange();

    // Responsible for handling state changes trihhered from moment-to-moment gameplay.
    public class GameStateManager
    {
        GameState m_current;
        GameState m_previous;

        public event OnGameStateTransition OnGameStateUpdate = default;
        public event OnTurnChange OnTurnChange = default;

        // Add all game states into the dictionary
        Dictionary<Type, GameState> m_gameStateCollection = new Dictionary<Type, GameState>
            {
                { typeof(TurnStart), new TurnStart() },
                { typeof(TurnEnd), new TurnEnd() },
                { typeof(PlayerTurn), new PlayerTurn() }
            };

        public void Init(List<GameEffect> startEffects = null)
        {
            SetCurrentState(typeof(TurnStart), startEffects);
        }

        public Type CurrentGameState() 
        {
            return m_current.GetType();
        }

        public void AddGameEffectsToState(Type gameState, List<GameEffect> gameEffects) 
        {
            m_gameStateCollection[gameState].AddEffects(gameEffects);
        }

        void SetCurrentState(Type type, List<GameEffect> gameEffects = null) 
        {
            m_current = m_gameStateCollection[type];
            m_current.Init(gameEffects);
            m_current.OnGameStateTransition += OnStateTransition;
            m_current?.OnEnter(m_previous);
        }

        void OnStateTransition(Type type) 
        {
            if (type == typeof(TurnStart))
            {
                OnTurnChange?.Invoke();
            }
            Debug.Log(type);
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
