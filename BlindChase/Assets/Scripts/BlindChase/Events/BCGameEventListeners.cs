using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Events
{
    // Container of all event responses for the needed game events.
    [Serializable]
    public class BCGameEventListeners : MonoBehaviour
    {
        public List<BCGameEventListener> m_eventListeners = new List<BCGameEventListener>();

        void OnEnable()
        {
            foreach (BCGameEventListener listener in m_eventListeners)
            {
                listener?.Init();
            }
        }

        void OnDisable()
        {
            foreach (BCGameEventListener listener in m_eventListeners)
            {
                listener?.Shutdown();
            }
        }
    }

}


