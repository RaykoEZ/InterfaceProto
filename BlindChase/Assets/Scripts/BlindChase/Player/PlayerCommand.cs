using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase 
{

    public class CommandArgs
    {
        public Dictionary<string, object> Arguments { get; private set; }

        public CommandArgs(Dictionary<string, object> args)
        {
            Arguments = args;
        }
    }

    public abstract class PlayerCommand
    {
        protected FactionMemberController m_controllerRef;
        public abstract void ExecuteCommand(CommandArgs args);

        public PlayerCommand(FactionMemberController controller) 
        {
            m_controllerRef = controller;
        }


    }

    public class MovePlayer : PlayerCommand 
    {

        public MovePlayer(FactionMemberController controller) : base(controller) 
        {      
        }

        public override void ExecuteCommand(CommandArgs args)
        {
            Dictionary<string, object> arg = args.Arguments;

            if (!arg.ContainsKey("destination") || !arg.ContainsKey("origin")) 
            {
                return;
            }

            Vector3 dest = (Vector3)arg["destination"];
            Vector3 origin = (Vector3) arg["origin"];

            if (dest == null || origin == null) 
            {
                return;
            }

            m_controllerRef.MovePlayer(dest, origin);

        }
    }

}

