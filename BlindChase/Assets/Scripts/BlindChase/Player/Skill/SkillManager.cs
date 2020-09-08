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
                SkillValueCollection skillData = skillDataColllection.ValueCollection;
                m_skillEffectCollection[skillData.AttributeId] = new SkillEffect(skillData.AttributeId);
            }
        }

        void SetSkillCooldown(CharacterState characterState, int skillId, int cooldown) 
        {
            characterState.CurrentSkillCooldowns[skillId] = cooldown;
        }

        public GameContextCollection ActivateSkill(int skillId, int skillLevel, GameContextCollection context, HashSet<TileId> targets, TileId userid) 
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
            int validSkillLevel = GetValidSkillLevel(skillDataColllection.ValueCollection.SkillValues.Count, skillLevel);

            // Set skill cooldown.
            int cooldown = skillDataColllection.ValueCollection.SkillValues[validSkillLevel].Cooldown;
            SetSkillCooldown(context.Characters.MemberDataContainer[userid].PlayerState, skillId, cooldown);

            SkillDataItem skillData = skillDataColllection.ValueCollection.SkillValues[validSkillLevel];
            SkillEffectArgs args = new SkillEffectArgs(context, targets, userid, skillData);

            GameContextCollection skillResults;
            skillResults = m_skillEffectCollection[skillDataColllection.ValueCollection.AttributeId].Activate(args);
                   
            return skillResults;
        }


        public int GetSkillTargetLimit(int skillId, int skillLevel) 
        {
            SkillDataCollection data = m_skillDatabase.GetSkill(skillId);
            int levelIndex = GetValidSkillLevel(data.ValueCollection.SkillValues.Count, skillLevel);

            return data.ValueCollection.SkillValues[levelIndex].TargetLimit;
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


