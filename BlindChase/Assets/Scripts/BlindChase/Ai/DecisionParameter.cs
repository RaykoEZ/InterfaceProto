using System;
using System.Collections.Generic;
using BlindChase.GameManagement;
using BlindChase.Utility;
using Newtonsoft.Json;

namespace BlindChase.Ai
{
    [Serializable]
    public enum NpcObjective
    {
        HOSTILITY,
        SURVIVAL
    }

    [Serializable]
    public class DecisionParameter 
    {
        [NonSerialized]
        Dictionary<NpcObjective, ObjectiveBias> m_objectiveUrgencies = new Dictionary<NpcObjective, ObjectiveBias>();
        [NonSerialized]
        bool m_isDirty = false;
        [NonSerialized]
        float m_biasSum = 0.0f;

        public NpcObjective MainObjective 
        {
            get 
            {
                NpcObjective ret = NpcObjective.SURVIVAL;
                float highestWeight = 0f;
                foreach(KeyValuePair<NpcObjective, ObjectiveBias> bias in ObjectiveUrgencies) 
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
        public Dictionary<NpcObjective, ObjectiveBias> ObjectiveUrgencies {
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
        protected DecisionParameter()
        {
            m_isDirty = true;
        }

        [JsonConstructor]
        public DecisionParameter(Dictionary<NpcObjective, ObjectiveBias> objectiveBiases)
        {
            m_objectiveUrgencies = objectiveBiases;
            m_isDirty = true;
        }

        // Get difference of two weights. 
        public float DiffWith(NpcObjective objType, float sourceWeight) 
        {
            float subtractBy = m_objectiveUrgencies.ContainsKey(objType)?
                m_objectiveUrgencies[objType].Weight : 0.0f;

            return sourceWeight - subtractBy;
        }


        public static DecisionParameter CalculateNewParameter(
            in GameContextRecord context,
            in CharacterState npcState,
            DecisionParameter oldDetails,
            ObservationResult observation) 
        {
            DecisionParameter newDetail = new DecisionParameter();
            foreach (KeyValuePair<NpcObjective, ObjectiveBias> objective in oldDetails.ObjectiveUrgencies)
            {
                float newPriority = CalculateUrgencyByType(context, objective, npcState, observation);
                newDetail.ObjectiveUrgencies[objective.Key] = new ObjectiveBias(newPriority);
            }
            return newDetail;
        }

        protected static float CalculateUrgencyByType(
            in GameContextRecord context,
            KeyValuePair<NpcObjective, ObjectiveBias> objective,
            CharacterState npcState,
            ObservationResult charactersInSight)
        {
            float defaultMod = objective.Value.Weight;
            float result = defaultMod;

            switch (objective.Key)
            {
                case NpcObjective.HOSTILITY:
                    {
                        CharacterContext character = context.CharacterRecord;
                        HashSet<string> factionIds = character.FactionIds;
                        // Remove the allied faction Id.
                        factionIds.Remove(npcState.ObjectId.FactionId);
                        float totslPower = 0f;
                        foreach(string factionId in factionIds) 
                        {
                            float enemyFactionPower = character.FactionPowerValue[factionId];
                            totslPower += enemyFactionPower;
                        }

                        float allyPower = character.FactionPowerValue[npcState.ObjectId.FactionId];

                        // Get enemy faction power factor.
                        float offenseFactor = (totslPower - allyPower) / totslPower;

                        int maxHp = npcState.Character.MaxHP;
                        int currentHp = npcState.CurrentHP;
                        result = PriorityUtil.AgressionPriority(defaultMod, offenseFactor, maxHp, currentHp);
                        break;
                    }
                case NpcObjective.SURVIVAL:
                    {
                        int maxHp = npcState.Character.MaxHP;
                        int currentHp = npcState.CurrentHP;
                        result = PriorityUtil.SurvivalPriority(defaultMod, maxHp, currentHp, charactersInSight.EnemyIds.Count, charactersInSight.AllyIds.Count);
                        break;
                    }
                default:
                    break;
            }

            return result;
        }
    }

}


