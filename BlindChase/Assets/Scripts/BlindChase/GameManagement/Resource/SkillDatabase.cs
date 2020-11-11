using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace BlindChase.GameManagement
{
    [CreateAssetMenu(fileName = "SkillDatabase", menuName = "BlindChase/Create collection of player skills", order = 1)]
    public class SkillDatabase : ScriptableObject 
    {
        Dictionary<int, SkillDataCollection> m_skillCollection;
        Dictionary<int, Sprite> m_skillIcons = new Dictionary<int, Sprite>();

        // File address for skill data storage
        const string c_skillFileRoot = "Skills/SkillCollection";
        const string c_skillIconRoot = "Skills/Art/Icons/";

        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter = 
            new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;
            TextAsset raw = Resources.Load<TextAsset>(c_skillFileRoot);

            m_skillCollection = JsonConvert.DeserializeObject<Dictionary<int, SkillDataCollection>>(raw.text,
                m_enumConverter);

            foreach (int skillId in m_skillCollection.Keys) 
            {
                Sprite skillIcon = Resources.Load<Sprite>(c_skillIconRoot + m_skillCollection[skillId].Image);
                m_skillIcons[skillId] = skillIcon;
            }
        }

        public SkillDataCollection GetSkill(int skillId) 
        {
            if (!m_skillCollection.ContainsKey(skillId)) 
            {
                return new SkillDataCollection();
            }

            return m_skillCollection[skillId];
        }

        public Sprite GetSkillIcon(int skillId)
        {
            if (!m_skillIcons.ContainsKey(skillId))
            {
                return null;
            }

            return m_skillIcons[skillId];
        }



    }

}


