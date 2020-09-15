using System;

namespace BlindChase.Events
{
    [Serializable]
    public class BCGameEventTrigger
    {
        public BCGameEventSource m_eventToTrigger = default;

        public void TriggerEvent(EventInfo eventInfo)
        {
            m_eventToTrigger?.Broadcast(eventInfo);
        }
    }

}


