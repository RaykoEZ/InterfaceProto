using System;
using System.Collections.Generic;
using BlindChase.GameManagement;
using BlindChase.Utility;
using Newtonsoft.Json;

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
    public class DecisionParameter 
    {
        [NonSerialized]
        Dictionary<NpcMainObjective, ObjectiveBias> m_objectiveUrgencies = new Dictionary<NpcMainObjective, ObjectiveBias>();
        [NonSerialized]
        bool m_isDirty = false;
        [NonSerialized]
        float m_biasSum = 0.0f;



        public NpcMainObjective MainObjective 
        {
            get 
            {
                NpcMainObjective ret = NpcMainObjective.SURVIVAL;
                float highestWeight = 0f;
                foreach(KeyValuePair<NpcMainObjective, ObjectiveBias> bias in ObjectiveUrgencies) 
                {
                    if(highestWeight < bias.Value.Weight) 
                    {
                        highestWeight = bias.Value.Weight;
                        ret = bias.Key;
                    }
                }
                return ret;
            }
        
        }


        // This modifies the derived priority values to show the nature of a faction's NPC.
        public Dictionary<NpcMainObjective, ObjectiveBias> ObjectiveUrgencies {
            get 
            {
                return m_objectiveUrgencies;
            } 
            set 
            {
                m_isDirty = true;
                m_objectiveUrgencies = value;
            } 
        }

        public float BiasSum 
        { 
            get 
            {
                if (m_isDirty && m_objectiveUrgencies.Count > 1) 
                {
                    m_biasSum = 0.0f;
                    foreach (ObjectiveBias objectiveWeight in ObjectiveUrgencies.Values)
                    {
                        m_biasSum += objectiveWeight.Weight;
                    }
                    m_isDirty = false;
                }

                return m_biasSum;
            } 
        }
        public DecisionParameter()
        {
            m_isDirty = true;
        }

        [JsonConstructor]
        public DecisionParameter(Dictionary<NpcMainObjective, ObjectiveBias> objectiveBiases)
        {
            m_objectiveUrgencies = objectiveBiases;
            m_isDirty = true;
        }

        public DecisionParameter DiffWith(DecisionParameter diffTarget) 
        {
            if(diffTarget == null) 
            {
                return null;
            }

            DecisionParameter ret = new DecisionParameter();
            foreach (KeyValuePair<NpcMainObjective, ObjectiveBias> objective in m_objectiveUrgencies)
            {
                float targetWeight = diffTarget.ObjectiveUrgencies.ContainsKey(objective.Key)? 
                    diffTarget.ObjectiveUrgencies[objective.Key].Weight : 0.0f;

                float weightDiff = targetWeight - m_objectiveUrgencies[objective.Key].Weight;

                ret.ObjectiveUrgencies[objective.Key] = new ObjectiveBias(weightDiff, objective.Value.Tolerance);
            }
            return ret;
        }


        public static DecisionParameter CalculateNewParameter(
            in DecisionParameter oldDetails, 
            in RangeMap visionRange,
            in RangeMap movementRange,
            in GameContextRecord context,
            in CharacterState npcState) 
        {
            DecisionParameter newDetail = new DecisionParameter();
            foreach (KeyValuePair<NpcMainObjective, ObjectiveBias> objective in oldDetails.ObjectiveUrgencies)
            {
                float newPriority = CalculateUrgencyByType(objective, visionRange, movementRange, context, npcState);
                newDetail.ObjectiveUrgencies[objective.Key] = new ObjectiveBias(newPriority, objective.Value.Tolerance);
            }
            return newDetail;
        }

        protected static float CalculateUrgencyByType(
            KeyValuePair<NpcMainObjective, ObjectiveBias> objective, 
            RangeMap visionRange,
            RangeMap movementRange,
            GameContextRecord context, 
            CharacterState npcState)
        {
            float defaultMod = objective.Value.Weight;
            float result = defaultMod;

            VisibleCharacters charactersInMoveRange = VisibleCharacters.GetVisibleCharacters(context, npcState, movementRange);
            VisibleCharacters charactersInSight = VisibleCharacters.GetVisibleCharacters(context, npcState, visionRange);

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
                        result = EvaluationHelper.SelfSurvivalPriority(defaultMod, maxHp, currentHp, charactersInSight.VisibleEnemies.Count, charactersInSight.VisibleAllies.Count);
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


