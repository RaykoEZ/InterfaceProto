using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public class CommandEvaluator
    {
        GameContextRecord m_baseContext;
        RangeOffsetMap m_movementRange;
        RangeOffsetMap m_visionRange;
        ObjectId m_npcId;
        RangeMapDatabase m_rangeDatabaseRef = default;

        public CommandEvaluator(RangeMapDatabase rangeDatabase)
        {
            m_rangeDatabaseRef = rangeDatabase;
        }

        public void Init(
            RangeOffsetMap movementRange,
            ObjectId npcObjectId,
            GameContextRecord context) 
        {
            m_npcId = npcObjectId;
            m_baseContext = context;
            m_visionRange = m_rangeDatabaseRef.GetSquareRadiusMap(2);
            m_movementRange = movementRange;
        }

        // gives a score between 0 and 1 for the given command
        public float Evaluate(in DecisionParameter decisionParam, NpcCommandResult result) 
        {
            GameContextRecord contextAfter = result.SimResult.ResultContext;

            // Perform various tests to obtain different scores on aspects of the state.
            CharacterState userAfterAction = contextAfter.CharacterRecord.MemberDataContainer[m_npcId].PlayerState;
            if (userAfterAction.IsDefeated)
            {
                return 0f;
            }

            float survivalRating = EvaluationUtil.SurvivalRating(m_npcId, m_baseContext.CharacterRecord, contextAfter.CharacterRecord);
            float attackRating = EvaluationUtil.AttackRating(m_npcId, result.SimResult, m_baseContext.CharacterRecord);

            ObservationResult newCharactersInSight = ObservationResult.CharactersInRange(
                contextAfter,
                m_rangeDatabaseRef,
                userAfterAction,
                m_visionRange);

            EvaluationUtil.EvaluatePositioning(userAfterAction, m_movementRange, newCharactersInSight, contextAfter,
                out float risk,
                out float advantage);

            float mainObjMod = 1.25f;
            float subObjMod = 0.75f;
            NpcObjective mainObjective = decisionParam.MainObjective;
            Debug.Log( $"{userAfterAction.Character.ClassType} A.I Main Obj: {mainObjective}");
            float ret = 0f;

            foreach (KeyValuePair<NpcObjective, ObjectiveBias> npcObjective in decisionParam.ObjectiveUrgencies) 
            {
                float objScore = GetResultScore(
                    npcObjective,
                    attackRating,
                    survivalRating,
                    risk,
                    advantage,
                    npcObjective.Key == mainObjective ? mainObjMod : subObjMod);

                ret += objScore;
            }

            int objCount = decisionParam.ObjectiveUrgencies.Count;
            if (objCount > 0) 
            {
                ret /= objCount;
            }     
            return ret;
        }

        float GetResultScore(
            KeyValuePair<NpcObjective, ObjectiveBias> objectiveBias, 
            float attackRating, 
            float survivalRating, 
            float risk, float advantage, 
            float scoreMod) 
        {
            // Use the obtained scores to derive resultScore, depending on the main objective.

            float baseScore = 0f;
            switch (objectiveBias.Key)
            {
                case NpcObjective.HOSTILITY:
                    baseScore = 0.8f * attackRating + 0.2f * survivalRating;
                    break;
                case NpcObjective.SURVIVAL:
                    // prioritise Vitality recovery and safety of current position.
                    baseScore = 0.3f * attackRating + 0.7f * survivalRating;
                    break;
                default:
                    break;
            }

            baseScore *= advantage / risk;
            baseScore *= scoreMod;
            return baseScore;
        }

    }

}


