using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public partial class DecisionHelper 
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

        public virtual CommandRequest MakeDecision(in DecisionParameter urgencyDetail) 
        {
            WorldContext world = m_baseContext.WorldRecord;
            List<NpcSimulationResult> possibleSkills = GetSkillOptions(urgencyDetail, world);

            CommandRequest bestCommand = AssessCommandOptions(urgencyDetail, possibleSkills);
            return bestCommand;
        }

        List<NpcSimulationResult> GetSkillOptions(in DecisionParameter urgencyDetail, in WorldContext world) 
        {
            List<NpcSimulationResult> possibleResults = new List<NpcSimulationResult>();

            foreach (IdLevelPair skillLevel in m_controllingStateRef.Character.SkillLevels)
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                bool preConditionsMet = SkillManager.CheckSkillPreconditions(m_controllingStateRef, skillLevel.Id, skillParam.SkillCost, out string message);
                if (preConditionsMet)
                {
                    List<Vector3Int> possibleTargets = GetSkillTargets(skillLevel.Id, m_skillRangesRef[skillLevel.Id], world);
                    int targetLimit = skillParam.TargetLimit;

                    NpcSimulationResult result = TryCommand(
                        urgencyDetail,
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
        NpcSimulationResult TryCommand(in DecisionParameter urgencyDetail, IdLevelPair kvp, int targetLimit, GameContextRecord context, List<Vector3Int> targets, ObjectId userId) 
        {
            NpcSimulationResult result;
            if (targetLimit > 1) 
            {
                SimulationResult triedResult = SkillManager.ActivateSkill(kvp.Id, kvp.Level, context, targets, userId);
                CharacterState newState = triedResult.ResulContext.CharacterRecord.MemberDataContainer[m_controllingStateRef.ObjectId].PlayerState;
                DecisionParameter newUrgency = DecisionParameter.CalculateNewParameter(urgencyDetail, m_visionRangeRef,m_movementRangeRef, triedResult.ResulContext, newState);
                NpcSimulationResult simResult = new NpcSimulationResult(triedResult, newUrgency);

                result = simResult;
            }
            // If npc needs to choose a target, get all possible results for each choice and choos the best one.
            else 
            {
                List<NpcSimulationResult> triedResults = new List<NpcSimulationResult>();

                foreach(Vector3Int target in targets) 
                {
                    SimulationResult triedResult = SkillManager.ActivateSkill(kvp.Id, kvp.Level, context, target, userId);
                    CharacterState newState = triedResult.ResulContext.CharacterRecord.MemberDataContainer[m_controllingStateRef.ObjectId].PlayerState;
                    DecisionParameter newUrgency = DecisionParameter.CalculateNewParameter(urgencyDetail, m_visionRangeRef, m_movementRangeRef, triedResult.ResulContext, newState);
                    NpcSimulationResult simResult = new NpcSimulationResult(triedResult, newUrgency);

                    triedResults.Add(simResult);
                }

                result = AssessResults(urgencyDetail, triedResults);
            }

            return result;
        }

        // Assess each possible results in a command's options, and pick the best one.
        NpcSimulationResult AssessResults(in DecisionParameter urgencyDetail, List<NpcSimulationResult> results)
        {
            // Return
            if(results.Count == 0) 
            {
                return null;
            }

            // We consider two options for best results:
            // - Best action to reduce Main Objective Urgency
            // - Best action to reduce Overal Urgency
            NpcMainObjective mainObjective = urgencyDetail.MainObjective;
            float bestMainObjDiff = 0f;
            float bestBiasSum = 0f;
            int bestMainObjIdx = 0;
            int bestOveralIdx = 0;
            // Get the best performing results' indices
            for (int i = 0; i < results.Count; ++i)
            {
                DecisionParameter diff = results[i].Parameters.DiffWith(urgencyDetail);
                float mainObjDiff = diff.ObjectiveUrgencies[mainObjective].Weight;
                // If a task decreases priority of a goal, it suggests this action accomplished the goal (to an extent).
                // So the more negative the diff is, the higher the scoring.
                if (bestMainObjDiff > mainObjDiff) 
                {
                    bestMainObjDiff = mainObjDiff;
                    bestMainObjIdx = i;
                }

                float objSum = diff.BiasSum;
                if (bestBiasSum > objSum)
                {
                    bestBiasSum = objSum;
                    bestOveralIdx = i;
                }
            }

            NpcSimulationResult bestResult = results[bestMainObjIdx];
            bool isBestDecided = bestOveralIdx == bestMainObjIdx;
            if (!isBestDecided) 
            {
                DecisionParameter mainObj = results[bestMainObjIdx].Parameters;
                float mainObjTolerance = urgencyDetail.ObjectiveUrgencies[mainObjective].Tolerance;
                // If urgency sum factor goes above tolerance, we are better choosing the best-overall instead.
                float performanceFactor = mainObj.BiasSum / (bestBiasSum + mainObj.BiasSum);
                // Higher tolerance => lower performance requirement for mainObj to be chosen.
                //** Tolerance converges to 1, performanceFactor also converges towards 1.
                bool isMainObjEffective = performanceFactor < mainObjTolerance;

                bestResult = isMainObjEffective ? results[bestMainObjIdx] : results[bestOveralIdx];
            }
   
            return bestResult;
        }


        // Assess each command's best result, and pick the best.
        CommandRequest AssessCommandOptions(DecisionParameter urgencyDetail, List<NpcSimulationResult> results)
        {
            NpcSimulationResult bestOption = AssessResults(urgencyDetail, results);


            return new CommandRequest();
        }

    }
}