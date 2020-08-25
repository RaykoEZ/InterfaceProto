using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase
{
    [CreateAssetMenu(fileName = "SkillDatabase", menuName = "BlindChase/Create collection of player skills", order = 1)]
    public class SkillDatabase : ScriptableObject 
    {
        Dictionary<string, SkillData> m_skillCollection = new Dictionary<string, SkillData>();
        
        // File address for skill data storage
        const string c_skillFileRoot = "";
        const string c_skillBaseValuesRoot = "";

        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter = 
            new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;
        }

        public SkillData GetSkill(string skillId) 
        {
            if (string.IsNullOrWhiteSpace(skillId) || !m_skillCollection.ContainsKey(skillId)) 
            {
                return null;
            }

            return m_skillCollection[skillId];
        }

        public void GetSkillDatabase() 
        {
        }

    }

}


