using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase
{
    public class SkillManager
    {
        static SkillDatabase m_skillDatabase = default;
        static Dictionary<SkillAttributeId, SkillEffect> m_skillEffectCollection = new Dictionary<SkillAttributeId, SkillEffect>();

        public SkillManager(HashSet<int> skillIds) 
        {
            m_skillDatabase = ScriptableObject.CreateInstance<SkillDatabase>();

            foreach(int skillId in skillIds) 
            {
                SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
                SkillValueCollection skillData = skillDataColllection.DataCollection;
                m_skillEffectCollection[skillData.AttributeId] = new SkillEffect(skillData.AttributeId);
            }
        }

        public void ProcessSkillEffect(int skillId, int skillLevel, Vector3Int targetPos, Vector3Int userPos)
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
            int validLevel = GetValidSkillLevel(skillDataColllection.DataCollection.SkillValues.Count, skillLevel);
            SkillDataItem skillData = skillDataColllection.DataCollection.SkillValues[validLevel];

            SkillEffectArgs args = new SkillEffectArgs(targetPos, userPos, skillData);
            
            m_skillEffectCollection[skillDataColllection.DataCollection.AttributeId].Activate(args);
        }

        public int GetSkillTargetLimit(int skillId, int skillLevel) 
        {
            SkillDataCollection data = m_skillDatabase.GetSkill(skillId);
            int levelIndex = GetValidSkillLevel(data.DataCollection.SkillValues.Count, skillLevel);

            return data.DataCollection.SkillValues[levelIndex].TargetLimit;
        }

        public static SkillDataCollection GetSkillData(int skillId)
        {
            return m_skillDatabase.GetSkill(skillId);
        }

        static int GetValidSkillLevel(int skillLevelLimit, int skillLevel) 
        {
            if(skillLevelLimit == 0) 
            {
                return 0;
            }

            return Mathf.Clamp(skillLevel, 0, skillLevelLimit - 1);

        }

    }

}


