using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public class DecisionHelper 
    {
        CharacterState m_targetStateRef;
        RangeMapDatabase m_rangeMapDatabaseRef = default;
        GameContextRecord m_baseContext;
        public void Init(RangeMapDatabase rangeMapDatabase) 
        {
            m_rangeMapDatabaseRef = rangeMapDatabase;
        }

        public virtual CommandRequest MakeDecision(
            NpcParameter priorityDetail,
            CharacterState npcState,
            RangeMap movementRange,
            GameContextRecord context) 
        {
            m_targetStateRef = npcState;
            m_baseContext = context;
            List<NpcCommandResult> skillResults = new List<NpcCommandResult>();
            // Find out the best skills to use/ whether we use any skills
            foreach (IdLevelPair skillLevel in m_targetStateRef.Character.SkillLevels)
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                int targetLimit = skillParam.TargetLimit;

                List<Vector3Int> targets = TargetSelection(skillLevel);
                CommandResult skillResult = TryCommand(skillLevel.Id, skillLevel.Level, m_baseContext, targets, m_targetStateRef.ObjectId);
            }

            return new CommandRequest();
        }

        List<Vector3Int> TargetSelection(IdLevelPair skillLevel) 
        {
            SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
            string skillRangeId = skillParam.EffectRange;
            int targetLimit = skillParam.TargetLimit;
            RangeMap range = m_rangeMapDatabaseRef.GetSkillRangeMap(skillRangeId);
            VisibleCharacters targets = VisibleCharacters.GetVisibleCharacters(m_baseContext, m_targetStateRef, range);

            // IMPL
            return new List<Vector3Int>();
        }
        // IMPL

        CommandResult TryCommand(int skillId, int skillLevel, GameContextRecord context, List<Vector3Int> targets, ObjectId userId) 
        {
            CommandResult result = SkillManager.ActivateSkill(skillId, skillLevel, context, targets, userId);
            return result;
        }
        // IMPL

        CommandResult AssessTargetOptions(NpcParameter priorityDetail, List<CommandResult> results)
        {
            return new CommandResult();
        }
        // IMPL

        CommandRequest AssessCommand(NpcParameter npcParam, List<NpcCommandResult> results) 
        {

            return new CommandRequest();
        }

        

    }
}


