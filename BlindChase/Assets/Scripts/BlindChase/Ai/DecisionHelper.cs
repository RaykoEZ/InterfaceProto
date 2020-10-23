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
        RangeMap m_movementRangeRef;
        RangeMap m_visionRangeRef;

        Dictionary<int, RangeMap> m_skillRangesRef;

        public void Setup(
             CharacterState npcState,
             RangeMap movementRange,
             RangeMap vision, 
             Dictionary<int, RangeMap> skillRange) 
        {
            m_movementRangeRef = movementRange;
            m_visionRangeRef = vision;
            m_skillRangesRef = skillRange;
            m_controllingStateRef = new CharacterState(npcState);
        }

        public virtual CommandRequest MakeDecision(in DecisionParameter urgencyDetail, GameContextRecord context) 
        {
            List<NpcSimulationResult> possibleSkills = GetBestSkillOptions(urgencyDetail, context);
            NpcSimulationResult bestAdvancingOption = GetBestMovementOption(urgencyDetail, context);
            possibleSkills.Add(bestAdvancingOption);

            CommandRequest bestCommand = AssessCommandOptions(urgencyDetail, possibleSkills);
            return bestCommand;
        }

        List<NpcSimulationResult> GetBestSkillOptions(in DecisionParameter urgencyDetail, GameContextRecord context) 
        {
            List<NpcSimulationResult> possibleResults = new List<NpcSimulationResult>();
            foreach (IdLevelPair skillLevel in m_controllingStateRef.Character.SkillLevels)
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                bool preConditionsMet = SkillManager.CheckSkillPreconditions(m_controllingStateRef, skillLevel.Id, skillParam.SkillCost, out string message);
                List<Vector3Int> possibleTargets = GetSkillTargets(skillLevel.Id, m_skillRangesRef[skillLevel.Id], context.WorldRecord);
                // Onl;y choose when you can afford the skill cost, skill is off Cooldown, and have valid targets.
                if (preConditionsMet && possibleTargets.Count > 0)
                {
                    int targetLimit = skillParam.TargetLimit;

                    NpcSimulationResult result = TrySkill(
                        urgencyDetail,
                        context,
                        skillLevel,
                        targetLimit,
                        possibleTargets);

                    possibleResults.Add(result);
                }
            }

            return possibleResults;
        }

        NpcSimulationResult GetBestMovementOption(in DecisionParameter urgencyDetail, GameContextRecord record) 
        {
            List<Vector3Int> targets = m_movementRangeRef.ApplyRangeOffsets(m_controllingStateRef.Position);
            List<Vector3Int> toRemove = new List<Vector3Int>();

            foreach (Vector3Int targetPos in targets)
            {
                bool isValid = TargetingValidation.IsDestinationValid(m_controllingStateRef.ObjectId, targetPos, record);
                if (!isValid)
                {
                    toRemove.Add(targetPos);
                }
            }

            foreach (Vector3Int removeThis in toRemove)
            {
                targets.Remove(removeThis);
            }

            return TryAdvancing(urgencyDetail, record, targets);
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
                bool isValid = TargetingValidation.IsSkillTargetSelectionValid(target, m_controllingStateRef.ObjectId, skillId);
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
        NpcSimulationResult TrySkill(
            in DecisionParameter urgencyDetail, 
            in GameContextRecord context, 
            IdLevelPair kvp, 
            int targetLimit, 
            List<Vector3Int> allTargets) 
        {
            NpcSimulationResult result;
            ObjectId userId = m_controllingStateRef.ObjectId;
            GameContextRecord inputContext = new GameContextRecord(context);
            SkillActivationInput inputRef = new SkillActivationInput(kvp.Id, kvp.Level, userId, inputContext);

            if (targetLimit > 1) 
            {
                SimulationResult triedResult = SkillManager.ActivateSkill(inputRef, allTargets);
                NpcSimulationResult simResult = NpcSimulationResult.Create(
                    CommandTypes.SKILL_ACTIVATE,
                    userId, 
                    urgencyDetail, 
                    m_visionRangeRef, 
                    m_movementRangeRef, 
                    triedResult,
                    allTargets,
                    inputRef);

                result = simResult;
            }
            // If npc needs to choose a target, get all possible results for each choice and choos the best one.
            else 
            {
                List<NpcSimulationResult> triedResults = new List<NpcSimulationResult>(allTargets.Count);

                foreach(Vector3Int target in allTargets) 
                {
                    SkillActivationInput inputCopy = new SkillActivationInput(inputRef);
                    SimulationResult triedResult = SkillManager.ActivateSkill(inputRef, target);
                    NpcSimulationResult simResult = NpcSimulationResult.Create(
                    CommandTypes.SKILL_ACTIVATE,
                    userId,
                    urgencyDetail,
                    m_visionRangeRef,
                    m_movementRangeRef,
                    triedResult,
                    target,
                    inputCopy);

                    triedResults.Add(simResult);
                }
                // Assess all results to pick the best one.
                result = AssessResults(urgencyDetail, triedResults);
            }

            return result;
        }

        NpcSimulationResult TryAdvancing(in DecisionParameter urgencyDetail, in GameContextRecord context, List<Vector3Int> allTargets) 
        {
            List<NpcSimulationResult> triedResults = new List<NpcSimulationResult>(allTargets.Count);

            foreach (Vector3Int pos in allTargets)
            {
                GameContextRecord simContext = new GameContextRecord(context);
                SimulationResult triedResult = SkillManager.BasicMovement(simContext, pos, m_controllingStateRef.ObjectId);
                NpcSimulationResult result = NpcSimulationResult.Create(
                    CommandTypes.ADVANCE,
                    m_controllingStateRef.ObjectId,
                    urgencyDetail,
                    m_visionRangeRef,
                    m_movementRangeRef,
                    triedResult,
                    pos);

                triedResults.Add(result);
            }

            // Assess all results to pick the best one.
            NpcSimulationResult bestResult = AssessResults(urgencyDetail, triedResults);

            return bestResult;
        }


        // Assess each possible results in a command's options, and pick the best one.
        NpcSimulationResult AssessResults(in DecisionParameter urgencyDetail, List<NpcSimulationResult> results)
        {
            // Return
            if(results.Count == 0) 
            {
                return null;
            }

            if(results.Count == 1) 
            {
                return results[0];
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
                DecisionParameter diff = results[i].Diff;
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

            DecisionParameter mainObj = results[bestMainObjIdx].Parameters;
            float mainObjTolerance = urgencyDetail.ObjectiveUrgencies[mainObjective].Tolerance;
            // If urgency sum factor goes above tolerance, we are better choosing the best-overall instead.
            float performanceFactor = mainObj.BiasSum / (bestBiasSum + mainObj.BiasSum);
            // Higher tolerance => lower performance requirement for mainObj to be chosen.
            //** Tolerance converges to 1, performanceFactor converges towards 1.
            // If MainObj Sum == BestOverall Sum, factor == 1/2, 
            // if factor > 0.5, MainObj is less effective overall than BestOverall, need tolerance to judge if we choose MainObj anyway.
            bool isMainObjEffective = performanceFactor < mainObjTolerance;

            NpcSimulationResult bestResult = isMainObjEffective ? results[bestMainObjIdx] : results[bestOveralIdx];
   
            return bestResult;
        }


        // Assess each command's best result, and pick the best.
        CommandRequest AssessCommandOptions(DecisionParameter urgencyDetail, List<NpcSimulationResult> results)
        {
            NpcSimulationResult bestOption = AssessResults(urgencyDetail, results);
            Dictionary<string, object> payload = new Dictionary<string, object>();

            switch (bestOption.CommandType)
            {
                case CommandTypes.ADVANCE:
                    {
                        // Selected location to move for advancement is always the first and only.
                        payload.Add("Destination", bestOption.Targets[0]);
                        break;
                    }
                case CommandTypes.SKILL_ACTIVATE:
                    {
                        payload.Add("SkillId", bestOption.SkillInput.SkillId);
                        payload.Add("SkillLevel", bestOption.SkillInput.SkillLevel);
                        payload.Add("Origin", m_controllingStateRef.Position);
                        HashSet<Vector3Int> targets = new HashSet<Vector3Int>(bestOption.Targets);
                        payload.Add("Target", targets);
                        break;
                    }
                default:
                    break;
            }

            CommandRequest ret = new CommandRequest(bestOption.CommandType, payload);

            return ret;
        }

    }
}