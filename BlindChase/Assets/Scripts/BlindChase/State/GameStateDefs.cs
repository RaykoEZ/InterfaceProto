using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BlindChase.State 
{
    // Store all defined game states here.

    public class GameStateDefs : ScriptableObject
    {

        Dictionary<Type, IGameState> m_gameStateCollection = 
            new Dictionary<Type, IGameState>();

        public void Init() 
        { 
        
        }

        public IGameState GetGameState(Type type) 
        {
            return m_gameStateCollection[type];
        }
    }
}
