using System;
using System.Collections.Generic;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    [Serializable]
    public enum NpcMainObjective
    {
        HOSTILITY,
        SURVIVAL,
        PROTECTION
    }


    [Serializable]
    public class NpcParameter 
    {
        [NonSerialized]
        Dictionary<NpcMainObjective, ObjectiveBias> m_objectiveBiases = new Dictionary<NpcMainObjective, ObjectiveBias>();
        [NonSerialized]
        bool m_isDirty = false;
        [NonSerialized]
        float m_biasSum = 0.0f;

        // This modifies the derived priority values to show the nature of a faction's NPC.
        public Dictionary<NpcMainObjective, ObjectiveBias> ObjectiveBiases {
            get 
            {
                return m_objectiveBiases;
            } 
            set 
            {
                m_isDirty = true;
                m_objectiveBiases = value;
            } 
        }

        public float BiasSum 
        { 
            get 
            {
                if (m_isDirty) 
                {
                    m_biasSum = 0.0f;
                    foreach (ObjectiveBias objectiveWeight in ObjectiveBiases.Values)
                    {
                        m_biasSum += objectiveWeight.Weight;
                    }
                    m_isDirty = false;
                }

                return m_biasSum;
            } 
        }

        public static NpcParameter CalculateNewParameter(
            NpcParameter oldDetails, 
            RangeMap observeRange, 
            GameContextRecord context,
            CharacterState npcState) 
        {
            NpcParameter newDetail = new NpcParameter();
            foreach (KeyValuePair<NpcMainObjective, ObjectiveBias> objective in oldDetails.ObjectiveBiases)
            {
                float newPriority = CalculatePriorityByType(objective, observeRange, context, npcState);
                newDetail.ObjectiveBiases[objective.Key] = new ObjectiveBias(newPriority, objective.Value.Tolerance);
            }
            return newDetail;
        }

        protected static float CalculatePriorityByType(
            KeyValuePair<NpcMainObjective, ObjectiveBias> objective, 
            RangeMap range, 
            GameContextRecord context, 
            CharacterState npcState)
        {
            float defaultMod = objective.Value.Weight;
            float result = defaultMod;

            VisibleCharacters charactersInMoveRange = VisibleCharacters.GetVisibleCharacters(context, npcState, range);
            switch (objective.Key)
            {
                case NpcMainObjective.HOSTILITY:
                    {
                        List<CharacterState> enemiesInRange = charactersInMoveRange.VisibleEnemies;
                        int numWeakEnemies = 0;
                        //Judge if any enemy looks weak.
                        foreach (CharacterState enemy in enemiesInRange)
                        {
                            bool isEnemyWeak = EvaluationHelper.
                                IsTargetWeak(enemy.CurrentHP, enemy.Character.MaxHP, objective.Value.Tolerance);
                            numWeakEnemies += isEnemyWeak ? 1 : 0;
                        }
                        int maxHp = npcState.Character.MaxHP;
                        int currentHp = npcState.CurrentHP;

                        result = EvaluationHelper.AgressionPriority(defaultMod, numWeakEnemies, maxHp, currentHp);
                        break;
                    }
                case NpcMainObjective.SURVIVAL:
                    {
                        int maxHp = npcState.Character.MaxHP;
                        int currentHp = npcState.CurrentHP;
                        result = EvaluationHelper.SelfSurvivalPriority(defaultMod, maxHp, currentHp, charactersInMoveRange.VisibleEnemies.Count, charactersInMoveRange.VisibleAllies.Count);
                        break;
                    }
                case NpcMainObjective.PROTECTION:
                    {
                        break;
                    }
                default:
                    break;
            }

            return result;
        }
    }

}


