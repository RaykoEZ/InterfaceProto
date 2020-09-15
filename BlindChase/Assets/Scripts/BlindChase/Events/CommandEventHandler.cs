using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase
{
    public class CommandEventHandler : MonoBehaviour
    {
        public event OnPlayerCommand<CommandEventInfo> OnCommand = default;

        public void OnPlayerMove(EventInfo info) 
        {
            CommandEventInfo commandInfo = 
                new CommandEventInfo(info.SourceId, CommandTypes.MOVE, info.Payload);

            OnCommand?.Invoke(commandInfo);
        }

        public void OnSkillPrompt(EventInfo info)
        {
            CommandEventInfo commandInfo =
                new CommandEventInfo(info.SourceId, CommandTypes.SKILL_PROMPT, info.Payload);
            OnCommand?.Invoke(commandInfo);
        }

        public void OnSkillConfirm(EventInfo info)
        {
            CommandEventInfo commandInfo =
                new CommandEventInfo(info.SourceId, CommandTypes.SKILL_ACTIVATE, info.Payload);
            OnCommand?.Invoke(commandInfo);
        }
    }

}


