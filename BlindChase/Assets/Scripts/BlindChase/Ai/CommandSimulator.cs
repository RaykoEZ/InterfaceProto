using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public partial class CommandSimulator 
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
            List<NpcCommandResult> possibleSkills = GetBestSkillOptions(urgencyDetail, context);
            NpcCommandResult bestAdvancingOption = GetBestMovementOption(urgencyDetail, context);
            possibleSkills.Add(bestAdvancingOption);

            CommandRequest bestCommand = AssessCommandOptions(urgencyDetail, possibleSkills);
            return bestCommand;
        }

        List<NpcCommandResult> GetBestSkillOptions(in DecisionParameter urgencyDetail, GameContextRecord context) 
        {
            List<NpcCommandResult> possibleResults = new List<NpcCommandResult>();
            foreach (SkillDataPair skillLevel in m_controllingStateRef.Character.SkillLevels)
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                bool preConditionsMet = SkillManager.CheckSkillPreconditions(m_controllingStateRef, skillLevel.Id, skillParam.SkillCost, out string message);

                List<Vector3Int> possibleTargets = TargetValidator.
                    GetSkillTargetOptions(
                    skillLevel.Id,
                    m_controllingStateRef.ObjectId, 
                    m_controllingStateRef.Position,
                    m_skillRangesRef[skillLevel.Id], 
                    context.WorldRecord);

                // Only choose when you can afford the skill cost, skill is off Cooldown, and have valid targets.
                if (preConditionsMet && possibleTargets.Count > 0)
                {
                    int targetLimit = skillParam.TargetLimit;

                    NpcCommandResult result = TrySkill(
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

        NpcCommandResult GetBestMovementOption(in DecisionParameter urgencyDetail, GameContextRecord record) 
        {
            List<Vector3Int> targets = TargetValidator.GetMovementOptions(
                m_controllingStateRef.ObjectId,
                m_controllingStateRef.Position,
                m_movementRangeRef,
                record
                );

            return TryAdvancing(urgencyDetail, record, targets);
        }

        // Get all possible results of using a command and return the best one.
        NpcCommandResult TrySkill(
            in DecisionParameter urgencyDetail, 
            in GameContextRecord context, 
            SkillDataPair kvp, 
            int targetLimit, 
            List<Vector3Int> allTargets) 
        {
            NpcCommandResult result;
            ObjectId userId = m_controllingStateRef.ObjectId;
            GameContextRecord inputContext = new GameContextRecord(context);
            SkillActivationInput inputRef = new SkillActivationInput(kvp.Id, kvp.Level, userId, inputContext);

            if (targetLimit > 1) 
            {
                SimulationResult triedResult = SkillManager.ActivateSkill(inputRef, allTargets);
                NpcCommandResult simResult = NpcCommandResult.Create(
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
                List<NpcCommandResult> triedResults = new List<NpcCommandResult>(allTargets.Count);

                foreach(Vector3Int target in allTargets) 
                {
                    SkillActivationInput inputCopy = new SkillActivationInput(inputRef);
                    GameManagement.SimulationResult triedResult = SkillManager.ActivateSkill(inputRef, target);
                    NpcCommandResult simResult = NpcCommandResult.Create(
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

        NpcCommandResult TryAdvancing(in DecisionParameter urgencyDetail, in GameContextRecord context, List<Vector3Int> allTargets) 
        {
            List<NpcCommandResult> triedResults = new List<NpcCommandResult>(allTargets.Count);

            foreach (Vector3Int pos in allTargets)
            {
                GameContextRecord simContext = new GameContextRecord(context);
                GameManagement.SimulationResult triedResult = SkillManager.BasicMovement(simContext, pos, m_controllingStateRef.ObjectId);
                NpcCommandResult result = NpcCommandResult.Create(
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
            NpcCommandResult bestResult = AssessResults(urgencyDetail, triedResults);

            return bestResult;
        }


        // Assess each possible results in a command's options, and pick the best one.
        NpcCommandResult AssessResults(in DecisionParameter urgencyDetail, List<NpcCommandResult> results)
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

            NpcMainObjective mainObjective = urgencyDetail.MainObjective;
            float bestMainObjDiff = urgencyDetail.ObjectiveUrgencies[mainObjective].Weight;
            float bestBiasSum = urgencyDetail.BiasSum;
            int bestMainObjIdx = 0;
            int bestOverallIdx = 0;
            // Get the best performing results' indices, the smaller the urgency weight, the better the option.
            for (int i = 0; i < results.Count; ++i)
            {
                float mainObjWeight = results[i].Parameters.ObjectiveUrgencies[mainObjective].Weight;

                // If a task decreases urgency of a goal, it suggests this action accomplished the goal (to an extent).
                // So the more negative the diff is, the higher the scoring.
                if (bestMainObjDiff > mainObjWeight) 
                {
                    bestMainObjDiff = mainObjWeight;
                    bestMainObjIdx = i;
                }

                // Checking for overall urgency.
                float objSum = results[i].Parameters.BiasSum;
                if (bestBiasSum > objSum)
                {
                    bestBiasSum = objSum;
                    bestOverallIdx = i;
                }
            }

            // If best overall is best for main objective, return the result now.
            if (bestMainObjIdx == bestOverallIdx) 
            {
                return results[bestMainObjIdx];
            }

            // We consider two options for best results:
            // - Best action to reduce Main Objective Urgency
            // - Best action to reduce Overal Urgency
            DecisionParameter mainObj = results[bestMainObjIdx].Parameters;
            // If urgency sum factor goes above tolerance, we are better choosing the best-overall instead.
            float performanceFactor = mainObj.BiasSum / (bestBiasSum + mainObj.BiasSum);

            // Higher tolerance => lower performance requirement for mainObj can be chosen.
            // If MainObj Sum == BestOverall Sum, factor == 1/2, 
            // if factor > 0.5, "MainObj" is less effective overall than "Best Overall", need tolerance to judge if we choose MainObj anyway.
            // Since tolerance ranges 0 to 1 and we want:  0.5 < tolerance < 1
            float mainObjTolerance = urgencyDetail.ObjectiveUrgencies[mainObjective].Tolerance;
            float tolerance = 0.5f + (0.5f * mainObjTolerance);
            bool isMainObjEffective = performanceFactor < tolerance;

            NpcCommandResult bestResult = isMainObjEffective ? results[bestMainObjIdx] : results[bestOverallIdx];
   
            return bestResult;
        }


        // Assess each command's best result, and pick the best.
        CommandRequest AssessCommandOptions(DecisionParameter urgencyDetail, List<NpcCommandResult> results)
        {
            NpcCommandResult bestOption = AssessResults(urgencyDetail, results);
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