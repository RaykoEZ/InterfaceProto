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

    public class CommandEventInfo : EventInfo
    {
        public Command Command { get; private set; }
        public CommandEventInfo(
            ObjectId tileId, 
            Command command,
            Action onFinishCallback = null) 
            : base(tileId, payload: null, onFinishCallback)
        {
            Command = command;
        }
    }

    public struct Command 
    {
        public CommandTypes CommandType;
        public Dictionary<string, object> CommandArgs;

        public Command(CommandTypes commandType, Dictionary<string, object> args = null) 
        {
            CommandType = commandType;
            CommandArgs = args;
        }
    }
    //
    public class CommandStackInfo : EventInfo 
    {
        public Stack<Command> CommandList { get; private set; }

        public CommandStackInfo(
        ObjectId tileId,
        Stack<Command> commands,
        Action onFinishCallback = null)
        : base(tileId, payload: null, onFinishCallback)
        {
            CommandList = commands;
        }
    }

}

