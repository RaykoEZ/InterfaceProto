using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;

namespace BlindChase.Ai
{
    public class NpcTaskPlanner
    {
        public event OnPlayerCommand<CommandStackInfo> OnNPCTaskPlanned = default;
        SkillManager m_skillManagerRef;
        RangeMapDatabase m_rangeDsplayMasks = default;
        NPCDetail m_defaultDetailRef;
        ObjectId m_activeNpcId;

        public void Shutdown() 
        {
            OnNPCTaskPlanned = null;
        }

        public void Init(SkillManager skillManager) 
        {
            m_skillManagerRef = skillManager;
            m_rangeDsplayMasks = ScriptableObject.CreateInstance<RangeMapDatabase>();
        }

        public void OnActive(ObjectId activateId, NPCDetail nature, GameContextCollection context) 
        {
            m_defaultDetailRef = nature;
            m_activeNpcId = activateId;
            CommandStackInfo result = EvaluatePriority(context);

            OnTaskPlanned(result);
        }

        CommandStackInfo EvaluatePriority(GameContextCollection context)
        {
            WorldContext world = context.World;
            CharacterContext characters = context.Characters;
            CharacterState self = characters.MemberDataContainer[m_activeNpcId].PlayerState;

            Stack<Command> commands = new Stack<Command>();
            Command move = new Command
            {
                CommandType = CommandTypes.ADVANCE,
                CommandArgs = new Dictionary<string, object>()
            };

            // movement test
            move.CommandArgs["Origin"] = world.WorldMap.GetCellCenterWorld(self.Position);
            move.CommandArgs["Destination"] = world.WorldMap.GetCellCenterWorld(self.Position + Vector3Int.up);

            commands.Push(move);

            CommandStackInfo result = new CommandStackInfo(m_activeNpcId, commands);          
            
            return result;
        }

        void OnTaskPlanned(CommandStackInfo info) 
        {
            OnNPCTaskPlanned?.Invoke(info);
        }
    }

}


