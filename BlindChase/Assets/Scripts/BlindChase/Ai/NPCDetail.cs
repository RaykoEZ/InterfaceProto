using System;
using System.Collections.Generic;

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
    public class NPCDetail 
    {
        [NonSerialized]
        Dictionary<NpcMainObjective, float> m_objectiveBiases;
        [NonSerialized]
        bool m_isDirty = false;
        [NonSerialized]
        float m_biasSum = 0.0f;

        // This modifies the derived priority values to show the nature of a faction's NPC.
        public Dictionary<NpcMainObjective, float> ObjectiveBiases {
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
                    foreach (float bias in ObjectiveBiases.Values)
                    {
                        m_biasSum += bias;
                    }
                    m_isDirty = false;
                }

                return m_biasSum;
            } 
        }

        public bool HasPriority(NpcMainObjective obj) 
        { 
            return ObjectiveBiases.ContainsKey(obj); 
        }

        public NPCDetail(Dictionary<NpcMainObjective, float> objectiveBiases) 
        {
            ObjectiveBiases = objectiveBiases;
            m_biasSum = BiasSum;
        }
    }

}


