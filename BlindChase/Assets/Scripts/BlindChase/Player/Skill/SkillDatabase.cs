using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase
{
    [CreateAssetMenu(fileName = "SkillDatabase", menuName = "BlindChase/Create collection of player skills", order = 1)]
    public class SkillDatabase : ScriptableObject 
    {
        Dictionary<int, SkillDataCollection> m_skillCollection;

        // File address for skill data storage
        const string c_skillFileRoot = "Skills/SkillCollection";

        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter = 
            new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;
            TextAsset raw = Resources.Load<TextAsset>(c_skillFileRoot);

            m_skillCollection = JsonConvert.DeserializeObject<Dictionary<int, SkillDataCollection>>(raw.text,
                m_enumConverter);

            Debug.Log(m_skillCollection.Values.Count);
        }

        public SkillDataCollection GetSkill(int skillId) 
        {
            if (!m_skillCollection.ContainsKey(skillId)) 
            {
                return new SkillDataCollection();
            }

            return m_skillCollection[skillId];
        }

        public void GetSkillAttributesFromSkill(int skillId) 
        {

        }

    }

}


