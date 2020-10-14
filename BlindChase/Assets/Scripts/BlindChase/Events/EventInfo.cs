using System;
using System.Collections.Generic;

namespace BlindChase.Events 
{
    [Serializable]
    public class EventInfo 
    {
        public ObjectId SourceId { get; protected set; }
        public Dictionary<string, object> Payload { get; protected set; }
        public Action OnFinishedCallback { get; protected set; }

        public EventInfo(ObjectId id, Dictionary<string, object> payload = null, Action onFinishCallback = null) 
        {
            SourceId = id;
            Payload = payload;
            OnFinishedCallback = onFinishCallback;
        }
    }

    public class CommandRequestInfo : EventInfo
    {
        public CommandRequest Command { get; private set; }
        public CommandRequestInfo(
            ObjectId tileId, 
            CommandRequest command,
            Action onFinishCallback = null,
            Dictionary<string, object> payload = null) 
            : base(tileId, payload: payload, onFinishCallback)
        {
            Command = command;
        }
    }

    public struct CommandRequest 
    {
        public CommandTypes CommandType;
        public Dictionary<string, object> CommandArgs;

        public CommandRequest(CommandTypes commandType, Dictionary<string, object> args = null) 
        {
            CommandType = commandType;
            CommandArgs = args;
        }
    }

}

