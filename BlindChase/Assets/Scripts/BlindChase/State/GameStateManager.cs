using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase.State 
{

    // Responsible for handling state changes trihhered from moment-to-moment gameplay.
    public class GameStateManager
    {
        IGameState m_current = null;
        IGameState m_previous = null;
        GameStateDefs m_gameStateDefs;

        public void Init() 
        {

            m_gameStateDefs = ScriptableObject.CreateInstance<GameStateDefs>();
            m_gameStateDefs.Init();
            HandleCurrentState();
            OnStateTransition();
        }

        public void HandleCurrentState() 
        {
            m_current.StateOps(m_previous);
        }

        public void OnStateTransition() 
        {            
            m_previous = m_current;
            m_current = m_gameStateDefs.GetGameState(m_current.TransitionTo());
        }

    }


}
