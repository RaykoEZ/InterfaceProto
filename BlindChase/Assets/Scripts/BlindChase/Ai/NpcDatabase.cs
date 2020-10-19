using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase.Ai
{
    [CreateAssetMenu(fileName = "NpcDatabase", menuName = "BlindChase/Create collection of NPC data", order = 1)]
    public class NpcDatabase : ScriptableObject
    {
        Dictionary<string, DecisionParameter> m_NpcData = default;

        const string c_npcDataRoot = "Ai/NpcDetail";

        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter =
           new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;
            TextAsset raw = Resources.Load<TextAsset>(c_npcDataRoot);
            m_NpcData = JsonConvert.DeserializeObject<Dictionary<string, DecisionParameter>>(raw.text,
            m_enumConverter);
        }

        public DecisionParameter GetNpcNature(string factionId)
        {
            if (string.IsNullOrWhiteSpace(factionId) || !m_NpcData.ContainsKey(factionId))
            {
                Debug.LogError("Npc Faction Id is not valid.");
                return null;
            }

            return m_NpcData[factionId];
        }
    }

}


