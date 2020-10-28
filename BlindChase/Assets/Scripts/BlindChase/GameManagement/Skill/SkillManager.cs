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
        public static SimulationResult ActivateSkill(in SkillActivationInput input, List<Vector3Int> targets) 
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(input.SkillId);
            int validSkillLevel = GetValidSkillLevel(skillDataColllection.ValueCollection.SkillValues.Count, input.SkillLevel);
            OnSPConsumption(skillDataColllection, input.SkillId, validSkillLevel, input);

            SkillParameters skillData = skillDataColllection.ValueCollection.SkillValues[validSkillLevel];
            List<CharacterState> affectedCharacters = new List<CharacterState>();
            SimulationResult skillResult = new SimulationResult();
            foreach (Vector3Int targetCoord in targets) 
            {
                SkillEffectArgs args = new SkillEffectArgs(input.Context, targetCoord, input.UserId, skillData);
                SimulationResult newResult = ActivateSkill_Internal(skillDataColllection.ValueCollection.AttributeId, args);
                // Add all affected characters
                affectedCharacters.AddRange(newResult.AffectedCharacters);
                skillResult = newResult;
            }

            skillResult.AffectedCharacters = affectedCharacters;
            return skillResult;
        }

        // Single target skill
        public static SimulationResult ActivateSkill(in SkillActivationInput input, Vector3Int target)
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(input.SkillId);
            int validSkillLevel = GetValidSkillLevel(skillDataColllection.ValueCollection.SkillValues.Count, input.SkillLevel);
            OnSPConsumption(skillDataColllection, input.SkillId, validSkillLevel, input);

            SkillParameters skillData = skillDataColllection.ValueCollection.SkillValues[validSkillLevel];
            SkillEffectArgs args = new SkillEffectArgs(input.Context, target, input.UserId, skillData);
            SimulationResult skillResult = ActivateSkill_Internal(skillDataColllection.ValueCollection.AttributeId, args);        
            return skillResult;
        }

        public static SimulationResult BasicMovement(in GameContextRecord context, Vector3Int targetCoord, ObjectId userId)
        {
            SkillEffectArgs args = new SkillEffectArgs(context, targetCoord, userId, null);
            SimulationResult result = ActivateSkill_Internal(SkillAttributeId.BasicMovement, args);
            return result;
        }

        public static SimulationResult AutoRecovery(in GameContextRecord context, ObjectId userId)
        {
            Vector3Int coord = context.CharacterRecord.MemberDataContainer[userId].PlayerState.Position;
            SkillEffectArgs args = new SkillEffectArgs(context, coord, userId, null);
            SimulationResult result = ActivateSkill_Internal(SkillAttributeId.AutoRecovery, args);
            return result;
        }

        public static bool CheckSkillPreconditions(CharacterState userState, int skillId, int cost, out string message) 
        {
            message = "";
            if (!userState.CurrentSkillCooldowns.ContainsKey(skillId))
            {
                message = "Invalid skill";
                return false;
            }

            // If skill on cooldown or skillId invalid, nope go back
            if (userState.CurrentSkillCooldowns[skillId] > 0)
            {
                message = "Skill still on cooldown.";
                //Prompt message : Skill on cooldown
                return false;
            }

            if (userState.CurrentSP < cost)
            {
                message = "Insufficient SP.";
                return false;
            }
            return true;
        }

        static void OnSPConsumption(in SkillDataCollection skillDataColllection, int skillId, int skillLevel, in SkillActivationInput input)
        {
            // Set skill cooldown.
            int cooldown = skillDataColllection.ValueCollection.SkillValues[skillLevel].Cooldown;
            // Deduct SP
            int skillCost = skillDataColllection.ValueCollection.SkillValues[skillLevel].SkillCost;
            input.OnSkillConsumption(skillId, cooldown, skillCost);
        }

        static SimulationResult ActivateSkill_Internal(SkillAttributeId effectId, SkillEffectArgs arg)
        {
            SimulationResult result = m_skillEffectCollection[effectId].Activate(arg);
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


