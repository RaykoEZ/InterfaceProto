using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public class SkillManager
    {
        static SkillDatabase m_skillDatabase = default;
        static Dictionary<SkillAttributeId, SkillEffect> m_skillEffectCollection = new Dictionary<SkillAttributeId, SkillEffect>();

        public void Init(HashSet<int> skillIds) 
        {
            m_skillDatabase = ScriptableObject.CreateInstance<SkillDatabase>();

            SkillAttributeId basicMovement = SkillAttributeId.BasicMovement;
            m_skillEffectCollection.Add(basicMovement, new SkillEffect(basicMovement));

            SkillAttributeId autoRecovery = SkillAttributeId.AutoRecovery;
            m_skillEffectCollection.Add(autoRecovery, new SkillEffect(autoRecovery));

            foreach (int skillId in skillIds) 
            {
                SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
                SkillValueCollection skillData = skillDataColllection.ValueCollection;
                m_skillEffectCollection[skillData.AttributeId] = new SkillEffect(skillData.AttributeId);
            }
        }

        // For multiple-target skills
        public static CommandResult ActivateSkill(int skillId, int skillLevel, GameContextRecord context, List<Vector3Int> targets, ObjectId userId) 
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
            int validSkillLevel = GetValidSkillLevel(skillDataColllection.ValueCollection.SkillValues.Count, skillLevel);
            CharacterState userState = context.CharacterRecord.MemberDataContainer[userId].PlayerState;
            OnSPConsumption(skillId, validSkillLevel, skillDataColllection, userState);

            SkillParameters skillData = skillDataColllection.ValueCollection.SkillValues[validSkillLevel];
            List<CharacterState> affectedCharacters = new List<CharacterState>();
            CommandResult skillResult = new CommandResult();
            foreach (Vector3Int targetCoord in targets) 
            {
                SkillEffectArgs args = new SkillEffectArgs(context, targetCoord, userId, skillData);
                CommandResult newResult = ActivateSkill_Internal(skillDataColllection.ValueCollection.AttributeId, args);
                // Add all affected characters
                affectedCharacters.AddRange(newResult.AffectedCharacters);
                skillResult = newResult;
            }

            skillResult.AffectedCharacters = affectedCharacters;
            return skillResult;
        }

        // Single target skill
        public static CommandResult ActivateSkill(int skillId, int skillLevel, GameContextRecord context, Vector3Int target, ObjectId userId)
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
            int validSkillLevel = GetValidSkillLevel(skillDataColllection.ValueCollection.SkillValues.Count, skillLevel);
            CharacterState userState = context.CharacterRecord.MemberDataContainer[userId].PlayerState;
            OnSPConsumption(skillId, validSkillLevel, skillDataColllection, userState);

            SkillParameters skillData = skillDataColllection.ValueCollection.SkillValues[validSkillLevel];
            SkillEffectArgs args = new SkillEffectArgs(context, target, userId, skillData);
            CommandResult skillResult = ActivateSkill_Internal(skillDataColllection.ValueCollection.AttributeId, args);        
            return skillResult;
        }

        public static CommandResult BasicMovement(GameContextRecord context, Vector3Int targetCoord, ObjectId userId)
        {
            SkillEffectArgs args = new SkillEffectArgs(context, targetCoord, userId, null);
            CommandResult result = ActivateSkill_Internal(SkillAttributeId.BasicMovement, args);
            return result;
        }

        public static CommandResult AutoRecovery(GameContextRecord context, ObjectId userId)
        {
            Vector3Int coord = context.CharacterRecord.MemberDataContainer[userId].PlayerState.Position;
            SkillEffectArgs args = new SkillEffectArgs(context, coord, userId, null);
            CommandResult result = ActivateSkill_Internal(SkillAttributeId.AutoRecovery, args);
            return result;
        }

        public static bool CheckSkillPreconditions(CharacterState userState, int skillId, int cost, out string message) 
        {
            message = "";
            if (!userState.CurrentSkillCooldowns.ContainsKey(skillId))
            {
                message = "Invalid skill";
                Debug.Log(message);
                return false;
            }

            // If skill on cooldown or skillId invalid, nope go back
            if (userState.CurrentSkillCooldowns[skillId] > 0)
            {
                message = "Skill still on cooldown.";
                //Prompt message : Skill on cooldown
                Debug.Log(message);
                return false;
            }

            if (userState.CurrentSP < cost)
            {
                message = "Insufficient SP.";
                Debug.Log(message);
                return false;
            }
            return true;
        }

        static void OnSPConsumption(int skillId, int skillLevel, SkillDataCollection skillDataColllection, CharacterState userState)
        {
            // Set skill cooldown.
            int cooldown = skillDataColllection.ValueCollection.SkillValues[skillLevel].Cooldown;
            // Deduct SP
            int skillCost = skillDataColllection.ValueCollection.SkillValues[skillLevel].SkillCost;

            userState.CurrentSkillCooldowns[skillId] = cooldown;
            userState.CurrentSP -= skillCost;
        }

        static CommandResult ActivateSkill_Internal(SkillAttributeId effectId, SkillEffectArgs arg)
        {
            CommandResult result = m_skillEffectCollection[effectId].Activate(arg);
            return result;
        }

        public static SkillDataCollection GetSkillData(int skillId)
        {
            return m_skillDatabase.GetSkill(skillId);
        }

        public static SkillParameters GetSkillParameters(int skillId, int skillLevel)
        {
            SkillDataCollection data = m_skillDatabase.GetSkill(skillId);
            int level = GetValidSkillLevel(data.ValueCollection.SkillValues.Count, skillLevel);
            return data.ValueCollection.SkillValues[level];
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


