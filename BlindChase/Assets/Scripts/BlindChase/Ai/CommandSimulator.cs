using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public class CommandSimulator 
    {
        CharacterState m_controllingStateRef;
        RangeMapDatabase m_rangeDatabaseRef = default;
        RangeOffsetMap m_movementRangeRef;
        Dictionary<int, RangeOffsetMap> m_skillRanges = new Dictionary<int, RangeOffsetMap>();

        CommandEvaluator m_evaluator;

        public CommandSimulator(RangeMapDatabase rangeDatabase) 
        {
            m_rangeDatabaseRef = rangeDatabase;
            m_evaluator = new CommandEvaluator(rangeDatabase);
        }

        public void Setup(
             CharacterState npcState,
             GameContextRecord context) 
        {
            m_controllingStateRef = new CharacterState(npcState);

            foreach (SkillDataPair skillLevel in m_controllingStateRef.Character.SkillLevels)
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                string rangeId = skillParam.EffectRange;
                m_skillRanges[skillLevel.Id] = m_rangeDatabaseRef.GetSkillRangeMap(rangeId);
            }

            m_movementRangeRef = m_rangeDatabaseRef.GetAttackRangeMap(m_controllingStateRef.Character.ClassType);
            m_evaluator.Init(m_movementRangeRef, m_controllingStateRef.ObjectId, context);
        }

        public virtual CommandRequest Simulate(in DecisionParameter urgencyDetail, GameContextRecord context) 
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

                List<Vector3Int> possibleTargets = TargetEvaluation.
                    InspectSkillTargetOptions(
                    skillLevel.Id,
                    m_controllingStateRef.ObjectId, 
                    m_controllingStateRef.Position,
                    m_skillRanges[skillLevel.Id], 
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
            HashSet<AdvancementOption> targets = TargetEvaluation.InspectMovementOption(
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
            IEnumerable<Vector3Int> allTargets) 
        {
            NpcCommandResult result;
            ObjectId userId = m_controllingStateRef.ObjectId;
            GameContextRecord inputContext = new GameContextRecord(context);
            SkillActivationInput inputRef = new SkillActivationInput(kvp.Id, kvp.Level, userId, inputContext);

            if (targetLimit > 1) 
            {
                SimulationResult triedResult = SkillManager.ActivateSkill(inputRef, allTargets);
                NpcCommandResult simResult = new NpcCommandResult(
                    CommandTypes.SKILL_ACTIVATE,
                    triedResult,
                    allTargets,
                    inputRef);

                result = simResult;
            }
            // If npc needs to choose a target, get all possible results for each choice and choos the best one.
            else 
            {
                List<NpcCommandResult> triedResults = new List<NpcCommandResult>();
                foreach(Vector3Int target in allTargets) 
                {
                    SkillActivationInput inputCopy = new SkillActivationInput(inputRef);
                    SimulationResult triedResult = SkillManager.ActivateSkill(inputRef, target);
                    NpcCommandResult simResult = new NpcCommandResult(
                    CommandTypes.SKILL_ACTIVATE,
                    triedResult,
                    target,
                    inputCopy);

                    triedResults.Add(simResult);
                }
                // Assess all results to pick the best one.
                result = AssessActions(urgencyDetail, triedResults);
            }

            return result;
        }

        NpcCommandResult TryAdvancing(in DecisionParameter urgencyDetail, in GameContextRecord context, IEnumerable<AdvancementOption> allTargets) 
        {
            List<NpcCommandResult> triedResults = new List<NpcCommandResult>();

            foreach (AdvancementOption target in allTargets)
            {
                GameContextRecord simContext = new GameContextRecord(context);
                SimulationResult triedResult = SkillManager.BasicMovement(simContext, target.Position, m_controllingStateRef.ObjectId);
                NpcCommandResult result = new NpcCommandResult(
                    CommandTypes.ADVANCE,
                    triedResult,
                    target.Position);

                triedResults.Add(result);
            }

            // Assess all results to pick the best one.
            NpcCommandResult bestResult = AssessActions(urgencyDetail, triedResults);

            return bestResult;
        }


        // Assess each possible results in a command's options, and pick the best one.
        NpcCommandResult AssessActions(in DecisionParameter urgencyDetail, List<NpcCommandResult> results)
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

            float bestScore = 0f;
            int bestScoringResultIdx = 0;
            // Get the best performing results' indices, the smaller the urgency weight, the better the option.
            for (int i = 0; i < results.Count; ++i)
            {
                // Evaluate the result and compare scores
                float score = m_evaluator.Evaluate(urgencyDetail, results[i]);

                if (bestScore < score) 
                {
                    bestScore = score;
                    bestScoringResultIdx = i;
                }
            }

            NpcCommandResult bestResult = results[bestScoringResultIdx];
   
            return bestResult;
        }


        // Assess each command's best result, and pick the best.
        CommandRequest AssessCommandOptions(DecisionParameter urgencyDetail, List<NpcCommandResult> results)
        {
            NpcCommandResult bestOption = AssessActions(urgencyDetail, results);
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