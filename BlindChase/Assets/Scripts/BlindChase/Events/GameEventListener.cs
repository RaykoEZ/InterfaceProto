using System;
using UnityEngine.Events;

namespace BlindChase.Events
{
    [Serializable]
    public class BCGameEvent : UnityEvent<EventInfo>
    { }

    [Serializable]
    public class BCGameEventListener
    {
        public BCGameEventSource m_eventToListen = default;
        public BCGameEvent m_responses = new BCGameEvent();

        public void Init()
        {
            m_eventToListen?.Listen(this);

        }

        public void Shutdown()
        {
            m_eventToListen?.Unlisten(this);

        }

        public void OnEventTriggered(EventInfo eventInfo)
        {
            m_responses?.Invoke(eventInfo);
        }
    }

}


