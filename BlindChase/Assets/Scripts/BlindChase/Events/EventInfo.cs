using System;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using System.ComponentModel;

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
        public object Payload { get; protected set; }
    }

    public class TileEventInfo : EventInfo
    {
        public TileId TileId { get; private set; }

        public Vector3 Location { get; private set; }

        public CommandTypes CommandType { get; private set;}

        public TileEventInfo(TileId tileId, Vector3 location, CommandTypes commandTypes = CommandTypes.NONE, object payload = null)
        {
            TileId = tileId;
            Location = location;
            CommandType = commandTypes;
            Payload = payload;
        }
    }
}

