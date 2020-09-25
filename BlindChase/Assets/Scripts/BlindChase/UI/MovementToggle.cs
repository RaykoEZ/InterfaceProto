using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BlindChase.Events;

namespace BlindChase
{
    public class MovementToggle : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_toggleText = default;
        [SerializeField] BCGameEventTrigger OnMovementPrompt = default;
        public void OnToggle(bool isOn) 
        {
            if (isOn)
            {
                // Turn into cancel button
                m_toggleText.text = "Cancel";
            }
            else 
            {
                m_toggleText.text = "Move";
            }

            Dictionary<string, object> payload = new Dictionary<string, object> { { "isMoving", isOn } };
            EventInfo info = new EventInfo(null, payload);
            OnMovementPrompt.TriggerEvent(info);
        }

    }

}


