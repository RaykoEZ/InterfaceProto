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
        protected PlayerController m_controllerRef;
        public abstract void ExecuteCommand(CommandArgs args);
        public virtual void ExpectData(CommandArgs args) { }
        public PlayerCommand(PlayerController controller) 
        {
            m_controllerRef = controller;
        }
    }


    public class MovePlayer : PlayerCommand 
    {
        public MovePlayer(PlayerController controller) : base(controller) 
        {      
        }

        public override void ExecuteCommand(CommandArgs args)
        {
            Dictionary<string, object> arg = args.Arguments;

            if (!arg.ContainsKey("Destination")) 
            {
                return;
            }

            Vector3 dest = (Vector3)arg["Destination"];

            if (dest == null) 
            {
                return;
            }

            m_controllerRef.MovePlayer(dest);

        }
    }

    public class SkillPrompt : PlayerCommand
    {
        public override void ExecuteCommand(CommandArgs args)
        {
            int skillId = (int)args.Arguments["SkillId"];
            int skillLevel = (int)args.Arguments["SkillLevel"];

            m_controllerRef.PromptSkillTargetSelection(skillId, skillLevel);
        }

        public SkillPrompt(PlayerController controller) : base(controller)
        {
        }
    }

    public class SkillActivate : PlayerCommand
    {
        int m_currentPromptingSkillId = -1;
        int m_currentPromptingSkillLevel = -1;

        public override void ExecuteCommand(CommandArgs args)
        {
            HashSet<Vector3> targetPos = (HashSet<Vector3>)args.Arguments["Target"];
            Vector3 userPos = (Vector3)args.Arguments["Origin"];

            m_controllerRef.ActivateSkill(m_currentPromptingSkillId, m_currentPromptingSkillLevel, targetPos, userPos);
            m_currentPromptingSkillId = -1;
            m_currentPromptingSkillLevel = -1;
        }

        public override void ExpectData(CommandArgs args) 
        {
            m_currentPromptingSkillId = (int)args.Arguments["SkillId"];
            m_currentPromptingSkillLevel = (int)args.Arguments["SkillLevel"];
        }

        public SkillActivate(PlayerController controller) : base(controller)
        {
        }
    }
}

