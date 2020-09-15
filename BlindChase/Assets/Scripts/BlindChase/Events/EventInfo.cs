﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Events 
{
    [Serializable]
    public class EventInfo 
    {
        public TileId SourceId { get; protected set; }
        public Dictionary<string, object> Payload { get; protected set; }

        public EventInfo(TileId id, Dictionary<string, object> payload = null) 
        {
            SourceId = id;
            Payload = payload;
        }
    }

    public class CommandEventInfo : EventInfo
    {
        public CommandTypes CommandType { get; private set;}

        public CommandEventInfo(
            TileId tileId, 
            CommandTypes commandTypes = CommandTypes.NONE, 
            Dictionary<string, object> payload = null) 
            : base(tileId, payload)
        {
            CommandType = commandTypes;
            Payload = payload;
        }
    }
}

