using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Events
{
    // Contains events that trigger from internal systems and listened to UI/player scripts
    // Idea inspired by Unite 2017 talk by Ryan Hipple
    // and improvements done by u/NeoDragonCP.
    [CreateAssetMenu(fileName = "GameEventSource", menuName = "BlindChase/Create a game event for things to listen.", order = 1)]
    public class BCGameEventSource : ScriptableObject
    {
        readonly HashSet<BCGameEventListener> m_eventListeners = new HashSet<BCGameEventListener>();

        public void Listen(BCGameEventListener callme)
        {
            m_eventListeners.Add(callme);
        }

        public void Unlisten(BCGameEventListener unCallme)
        {
            if (m_eventListeners.Contains(unCallme))
            {
                m_eventListeners.Remove(unCallme);
            }
        }

        public void Broadcast(EventInfo eventInfo)
        {
            foreach (BCGameEventListener listener in m_eventListeners)
            {
                listener.OnEventTriggered(eventInfo);
            }

        }
    }
}


