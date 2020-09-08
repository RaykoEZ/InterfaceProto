using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Events 
{
    public class BCEventArgs<T> where T : EventInfo
    { 
        public T EventInfo { get; private set; }

        public BCEventArgs(T info) 
        {
            EventInfo = info;
        }
    }

    public abstract class EventInfo 
    {
        public Dictionary<string, object> Payload { get; protected set; }
    }

    public class CommandEventInfo : EventInfo
    {
        public TileId TileId { get; private set; }

        public Vector3 Location { get; private set; }

        public CommandTypes CommandType { get; private set;}

        public CommandEventInfo(TileId tileId, Vector3 location, CommandTypes commandTypes = CommandTypes.NONE, Dictionary<string, object> payload = null)
        {
            TileId = tileId;
            Location = location;
            CommandType = commandTypes;
            Payload = payload;
        }
    }
}

