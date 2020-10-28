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

        // Get difference of two weights. 
        public float DiffWith(NpcMainObjective objType, float sourceWeight) 
        {
            float subtractBy = m_objectiveUrgencies.ContainsKey(objType)?
                m_objectiveUrgencies[objType].Weight : 0.0f;

            return sourceWeight - subtractBy;
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

                        float offenseScore = 0;
                        //Judge if any enemy looks weak.
                        foreach (CharacterState enemy in enemiesInRange)
                        {
                            TargetValidator.IsAttackTargetValid(
                                npcState.ObjectId, 
                                enemy.Position, 
                                context, 
                                out bool isAttackable, 
                                out bool isDefeatable);

                            offenseScore += isAttackable ? 0.5f : 0;
                            offenseScore += isDefeatable ? 0.5f : 0;
                        }

                        int maxHp = npcState.Character.MaxHP;
                        int currentHp = npcState.CurrentHP;
                        // Get average offense score per target option.
                        float offenseFactor = enemiesInRange.Count == 0 ? 1.0f : offenseScore / enemiesInRange.Count;
                        result = EvaluationUtil.AgressionPriority(defaultMod, offenseFactor, maxHp, currentHp);
                        break;
                    }
                case NpcMainObjective.SURVIVAL:
                    {
                        int maxHp = npcState.Character.MaxHP;
                        int currentHp = npcState.CurrentHP;
                        result = EvaluationUtil.SelfSurvivalPriority(defaultMod, maxHp, currentHp, charactersInSight.VisibleEnemies.Count, charactersInSight.VisibleAllies.Count);
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


