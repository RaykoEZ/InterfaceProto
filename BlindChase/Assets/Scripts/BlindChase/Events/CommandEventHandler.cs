using UnityEngine;

namespace BlindChase.Events
{
    public class CommandEventHandler : MonoBehaviour
    {
        public event OnPlayerCommand<CommandRequestInfo> OnCommand = default;

        // These should explicitly take CommandEventInfo in the future.
        public void OnPlayerMove(EventInfo info) 
        {
            CommandRequestInfo commandInfo = info is CommandRequestInfo commandEventInfo ?
               commandEventInfo :
               new CommandRequestInfo(info.SourceId, new CommandRequest(CommandTypes.ADVANCE, info.Payload));

            OnCommand?.Invoke(commandInfo);
        }

        public void OnSkillPrompt(EventInfo info)
        {
            CommandRequestInfo commandInfo = info is CommandRequestInfo commandEventInfo ?
               commandEventInfo :
               new CommandRequestInfo(info.SourceId, new CommandRequest(CommandTypes.SKILL_PROMPT, info.Payload));

            OnCommand?.Invoke(commandInfo);
        }

        public void OnSkillConfirm(EventInfo info)
        {
            CommandRequestInfo commandInfo = info is CommandRequestInfo commandEventInfo ? 
                commandEventInfo : 
                new CommandRequestInfo(info.SourceId, new CommandRequest(CommandTypes.SKILL_ACTIVATE, info.Payload));

            OnCommand?.Invoke(commandInfo);
        }
    }

}


