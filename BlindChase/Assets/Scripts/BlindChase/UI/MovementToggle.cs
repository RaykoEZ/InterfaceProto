using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlindChase.Events;

namespace BlindChase.UI
{
    public class MovementToggle : MonoBehaviour
    {
        [SerializeField] Toggle m_toggle = default;
        [SerializeField] TextMeshProUGUI m_toggleText = default;
        [SerializeField] BCGameEventTrigger OnMovementPrompt = default;
        public void OnToggle(bool isOn) 
        {
            SetToggleText(isOn);

            Dictionary<string, object> payload = new Dictionary<string, object> { { "isMoving", isOn } };
            EventInfo info = new EventInfo(null, payload);
            TriggerPrompt(info);
        }

        void SetToggleText(bool isOn) 
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
        }

        void TriggerPrompt(EventInfo info) 
        {
            OnMovementPrompt.TriggerEvent(info);
        }

        public void OnMovementCancelled(EventInfo info) 
        {
            bool isCancelForMovement = (info is CommandRequestInfo commandInfo && commandInfo.Command.CommandType == CommandTypes.ADVANCE);
            m_toggle.isOn = isCancelForMovement && m_toggle.isOn;
        }

    }

}


