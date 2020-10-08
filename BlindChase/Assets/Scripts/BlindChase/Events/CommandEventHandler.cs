using UnityEngine;

namespace BlindChase.Events
{
    public class CommandEventHandler : MonoBehaviour
    {
        public event OnPlayerCommand<CommandEventInfo> OnCommand = default;

        // These should explicitly take CommandEventInfo in the future.
        public void OnPlayerMove(EventInfo info) 
        {
            CommandEventInfo commandInfo = info is CommandEventInfo commandEventInfo ?
               commandEventInfo :
               new CommandEventInfo(info.SourceId, new Command(CommandTypes.ADVANCE, info.Payload));

            OnCommand?.Invoke(commandInfo);
        }

        public void OnSkillPrompt(EventInfo info)
        {
            CommandEventInfo commandInfo = info is CommandEventInfo commandEventInfo ?
               commandEventInfo :
               new CommandEventInfo(info.SourceId, new Command(CommandTypes.SKILL_PROMPT, info.Payload));

            OnCommand?.Invoke(commandInfo);
        }

        public void OnSkillConfirm(EventInfo info)
        {
            CommandEventInfo commandInfo = info is CommandEventInfo commandEventInfo ? 
                commandEventInfo : 
                new CommandEventInfo(info.SourceId, new Command(CommandTypes.SKILL_ACTIVATE, info.Payload));

            OnCommand?.Invoke(commandInfo);
        }
    }

}


