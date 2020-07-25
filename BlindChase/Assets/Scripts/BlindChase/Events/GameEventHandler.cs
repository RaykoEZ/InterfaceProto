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
        public Vector3 Location { get; private set; }

        public TileEventInfo(Vector3 location, object payload = null)
        {
            Location = location;
            Payload = payload;
        }
    }

    // Handling events coming from tiles displayed in game.
    public delegate void GameTileEvent(BCEventArgs<TileEventInfo> EventArgs);
    public class GameEventHandler
    {
        public GameTileEvent GameTileEventTriggered = default;


        public void Init(ITileManager tileManager) 
        {
            tileManager.OnTileEvent += OnTileEvent;
        }

        void OnTileEvent(TileEventInfo info)
        {
            BCEventArgs<TileEventInfo> arg = new BCEventArgs<TileEventInfo>(info);

            GameTileEventTriggered?.Invoke(arg);
        }

    }
}

