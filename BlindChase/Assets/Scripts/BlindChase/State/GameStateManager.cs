using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase.State
{
    // Listens to possible events to change game environment properties / end the game. 
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] BCGameEventTrigger OnGameEnd = default;

        // Need Rules to be implemented and Additional Rules 
        // to be stored and accessed through reflection when loading the level
        public void Init() 
        {       
        }

        public void OnCharacterDefeated(EventInfo info) 
        {       
        }

        public void OnLeaderDeeated(EventInfo info) 
        {
            OnGameEnd?.TriggerEvent(info);
        }
    }

}


