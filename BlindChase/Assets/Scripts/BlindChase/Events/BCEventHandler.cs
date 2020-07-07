using System;
using UnityEngine;
using UnityEditor.MemoryProfiler;
using System.ComponentModel;

namespace BlindChase.Events 
{
    public class BCEventArgs : EventArgs 
    { 
        public CommandTypes CommandType { get; private set; }

        public object Data { get; private set; }

        public BCEventArgs(CommandTypes command, object data) 
        {
            CommandType = command;
            Data = data;
        }
    }



    public class BCEventHandler
    {
        public event EventHandler<BCEventArgs> GameEventTriggered = default;
    }
}

