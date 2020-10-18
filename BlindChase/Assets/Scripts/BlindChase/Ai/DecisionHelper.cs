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
        CharacterState m_controllingStateRef;
        GameContextRecord m_baseContext;
        RangeMap m_movementRangeRef;
        RangeMap m_visionRangeRef;

        Dictionary<int, RangeMap> m_skillRangesRef;

        public void Setup(
            RangeMap movementRange,
            RangeMap vision, 
            Dictionary<int, RangeMap> skillRange, 
            CharacterState npcState, 
            GameContextRecord record) 
        {
            m_movementRangeRef = movementRange;
            m_visionRangeRef = vision;
            m_skillRangesRef = skillRange;
            m_controllingStateRef = npcState;
            m_baseContext = record;
        }

        public virtual CommandRequest MakeDecision(NpcParameter priorityDetail) 
        {
            WorldContext world = m_baseContext.WorldRecord;
            List<CommandResult> possibleSkills = GetSkillOptions(priorityDetail, world);

            CommandRequest bestCommand = AssessCommandOptions(priorityDetail, possibleSkills);
            return bestCommand;
        }

        List<CommandResult> GetSkillOptions(NpcParameter priorityDetail, WorldContext world) 
        {
            List<CommandResult> possibleResults = new List<CommandResult>();

            foreach (IdLevelPair skillLevel in m_controllingStateRef.Character.SkillLevels)
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                bool preConditionsMet = SkillManager.CheckSkillPreconditions(m_controllingStateRef, skillLevel.Id, skillParam.SkillCost, out string message);
                if (preConditionsMet)
                {
                    List<Vector3Int> possibleTargets = GetSkillTargets(skillLevel.Id, m_skillRangesRef[skillLevel.Id], world);
                    int targetLimit = skillParam.TargetLimit;

                    CommandResult result = TryCommand(
                        priorityDetail,
                        skillLevel,
                        targetLimit,
                        m_baseContext,
                        possibleTargets,
                        m_controllingStateRef.ObjectId);

                    possibleResults.Add(result);
                }
            }

            return possibleResults;
        }


        // Get all valid targets for a command.
        List<Vector3Int> GetSkillTargets(int skillId, RangeMap range, WorldContext world) 
        {
            List<Vector3Int> allTargets = range.ApplyRangeOffsets(m_controllingStateRef.Position);
            List<Vector3Int> toRemove = new List<Vector3Int>();
            // Check and remove any invalid targets.
            foreach (Vector3Int targetPos in allTargets) 
            {
                ObjectId target = world.GetOccupyingTileAt(targetPos);
                AllowedTarget targetRule = TargetingValidation.GetSelectionType(target, m_controllingStateRef.ObjectId);
                bool isValid = TargetingValidation.IsSkillTargetSelectionValid(skillId, targetRule);
                if (!isValid) 
                {
                    toRemove.Add(targetPos);
                }
            }

            foreach(Vector3Int removeThis in toRemove) 
            {
                allTargets.Remove(removeThis);
            }
            
            return allTargets;
        }

        // Get all possible results of using a command and return the best one.
        CommandResult TryCommand(NpcParameter priorityDetail, IdLevelPair kvp, int targetLimit, GameContextRecord context, List<Vector3Int> targets, ObjectId userId) 
        {
            CommandResult result;
            if (targetLimit > 1) 
            {
                result = SkillManager.ActivateSkill(kvp.Id, kvp.Level, context, targets, userId);
            }
            else 
            {
                List<CommandResult> allResults = new List<CommandResult>(targets.Count);
                foreach(Vector3Int target in targets) 
                {
                    CommandResult triedResult = SkillManager.ActivateSkill(kvp.Id, kvp.Level, context, target, userId);
                    allResults.Add(triedResult);
                }

                result = AssessResults(priorityDetail, allResults);
            }

            return result;
        }

        // Assess each possible results in a command's options, and pick the best one.
        CommandResult AssessResults(NpcParameter priorityDetail, List<CommandResult> results)
        {
            List<NpcParameter> newParamCollection = new List<NpcParameter>(results.Count);
            foreach(CommandResult option in results)
            {
                CharacterState newState = option.ResulContext.CharacterRecord.MemberDataContainer[m_controllingStateRef.ObjectId].PlayerState;
                NpcParameter newParam = NpcParameter.CalculateNewParameter(priorityDetail, m_visionRangeRef, option.ResulContext, newState);
                newParamCollection.Add(newParam);
            }



            return new CommandResult();
        }

        // Assess each command's best result, and pick the best.
        CommandRequest AssessCommandOptions(NpcParameter priorityDetail, List<CommandResult> results)
        {
            return new CommandRequest();
        }

    }
}