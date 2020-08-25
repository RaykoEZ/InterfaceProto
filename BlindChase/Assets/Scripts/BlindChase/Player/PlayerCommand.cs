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
        protected TileController m_controllerRef;
        public abstract void ExecuteCommand(CommandArgs args);

        public PlayerCommand(TileController controller) 
        {
            m_controllerRef = controller;
        }


    }

    public class MovePlayer : PlayerCommand 
    {

        public MovePlayer(TileController controller) : base(controller) 
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

    public class Skill : PlayerCommand
    {
        public string SkillId { get; private set; }
        public TileId User { get; private set; }
        public List<TileId> Targets { get; private set; }

        public override void ExecuteCommand(CommandArgs args) 
        {
            string id = args.Arguments["SkillId"].ToString();
            TileId user = (TileId)args.Arguments["User"];
            List<TileId> targets = (List<TileId>) args.Arguments["Targets"];

            SkillId = id;
            User = user;
            SetTargets(targets);


        }

        public Skill(TileController controller) : base(controller)
        {
        }

        public virtual void SetTargets(List<TileId> targets)
        {
            Targets = targets;
        }
    }

}

