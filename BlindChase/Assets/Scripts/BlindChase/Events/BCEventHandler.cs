using System;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using System.ComponentModel;

namespace BlindChase.Events 
{
    public class BCEventArgs : EventArgs 
    { }

    public class PlayerPositionEventArgs : BCEventArgs
    {
        Vector3Int Coord;
    }

    public class PlayerObjectEventArgs : BCEventArgs
    {
        GameObject Player;
    }

    public class WorldEventArgs : BCEventArgs
    {

    }


    public class BCEventHandler
    {
        public event EventHandler<PlayerPositionEventArgs> PlayerMoved = default;

        public event EventHandler<PlayerObjectEventArgs> PlayerRefChanged = default;

        public event EventHandler<WorldEventArgs> WorldChanged = default;

    }
}

