using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase.GameManagement 
{

    public class CommandArgs
    {
        public Dictionary<string, object> Arguments { get; private set; }

        public CommandArgs(Dictionary<string, object> args)
        {
            Arguments = args;
        }
    }

    public abstract class CharacterCommand
    {
        protected BCCharacterController m_controllerRef;

        protected Action m_onCommandFinish = default;

        public virtual void ExecuteCommand(CommandArgs args, Action onFinishCallback = null) 
        {
            m_onCommandFinish = onFinishCallback;
        }

        public virtual void ExpectData(CommandArgs args) { }
        public CharacterCommand(BCCharacterController controller) 
        {
            m_controllerRef = controller;
            m_controllerRef.OnCommandFinish += OnFinish;
        }

        protected virtual void OnFinish() 
        {
            m_onCommandFinish?.Invoke();
        }

        public virtual void Shutdown() 
        {
            m_controllerRef.OnCommandFinish -= OnFinish;
        }
    }


    public class MovePlayer : CharacterCommand 
    {        

        public MovePlayer(BCCharacterController controller) : base(controller) 
        {
        }

        public override void ExecuteCommand(CommandArgs args, Action onFinishCallback = null)
        {
            base.ExecuteCommand(args, onFinishCallback);
            Dictionary<string, object> arg = args.Arguments;

            if (arg == null || !arg.ContainsKey("Destination")) 
            {
                return;
            }

            Vector3Int dest = (Vector3Int)arg["Destination"];

            if (dest == null) 
            {
                return;
            }

            m_controllerRef.AdvancePlayer(dest);

        }
    }

    public class SkillPrompt : CharacterCommand
    {
        public SkillPrompt(BCCharacterController controller) : base(controller)
        {
        }

        public override void ExecuteCommand(CommandArgs args, Action onFinishCallback = null)
        {
            base.ExecuteCommand(args, onFinishCallback);

            int skillId = (int)args.Arguments["SkillId"];
            int skillLevel = (int)args.Arguments["SkillLevel"];

            m_controllerRef.PromptSkillTargetSelection(skillId, skillLevel);
        }


    }

    public class SkillActivate : CharacterCommand
    {
        int m_currentPromptingSkillId = -1;
        int m_currentPromptingSkillLevel = -1;
        public SkillActivate(BCCharacterController controller) : base(controller)
        {
        }

        public override void ExecuteCommand(CommandArgs args, Action onFinishCallback = null)
        {
            base.ExecuteCommand(args, onFinishCallback);

            HashSet<Vector3Int> targetPos = (HashSet<Vector3Int>)args.Arguments["Target"];
            Vector3Int userPos = (Vector3Int)args.Arguments["Origin"];

            if(args.Arguments.ContainsKey("SkillId") && args.Arguments.ContainsKey("SkillLevel")) 
            {
                ExpectData(args);
            }

            m_controllerRef.ActivateSkill(m_currentPromptingSkillId, m_currentPromptingSkillLevel, targetPos, userPos);
            m_currentPromptingSkillId = -1;
            m_currentPromptingSkillLevel = -1;
        }

        public override void ExpectData(CommandArgs args) 
        {
            m_currentPromptingSkillId = (int)args.Arguments["SkillId"];
            m_currentPromptingSkillLevel = (int)args.Arguments["SkillLevel"];
        }


    }
}

