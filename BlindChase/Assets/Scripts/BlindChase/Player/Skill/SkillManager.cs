﻿using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase
{
    public class SkillManager
    {
        static SkillDatabase m_skillDatabase = default;
        static Dictionary<SkillAttributeId, SkillEffect> m_skillEffectCollection = new Dictionary<SkillAttributeId, SkillEffect>();
        public event OnCharacterDefeated OnCharacterDefeat = default;

        public SkillManager(HashSet<int> skillIds) 
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

        void OnSPConsumption(CharacterState characterState, int skillId, int cooldown, int cost) 
        {
            characterState.CurrentSkillCooldowns[skillId] = cooldown;
            characterState.CurrentSP -= cost;
        }

        public EffectResult ActivateSkill(int skillId, int skillLevel, GameContextCollection context, List<Vector3Int> targets, TileId userId) 
        {
            SkillDataCollection skillDataColllection = m_skillDatabase.GetSkill(skillId);
            int validSkillLevel = GetValidSkillLevel(skillDataColllection.ValueCollection.SkillValues.Count, skillLevel);

            CharacterState playerState = context.Characters.MemberDataContainer[userId].PlayerState;
            // Set skill cooldown.
            int cooldown = skillDataColllection.ValueCollection.SkillValues[validSkillLevel].Cooldown;
            // Deduct SP
            int skillCost = skillDataColllection.ValueCollection.SkillValues[validSkillLevel].SkillCost;
            OnSPConsumption(playerState, skillId, cooldown, skillCost);

            SkillDataItem skillData = skillDataColllection.ValueCollection.SkillValues[validSkillLevel];

            EffectResult skillResult = new EffectResult();
            GameContextCollection contextCollection = context;
            List<CharacterState> affectedCharacters = new List<CharacterState>();

            foreach (Vector3Int targetCoord in targets) 
            {
                SkillEffectArgs args = new SkillEffectArgs(contextCollection, targetCoord, userId, skillData);
                EffectResult newResult = ActivateSkill_Internal(skillDataColllection.ValueCollection.AttributeId, args);
                // Add all affected characters
                affectedCharacters.AddRange(newResult.AffectedCharacters);
                skillResult = newResult;
            }


            skillResult.AffectedCharacters = affectedCharacters;
            return skillResult;
        }

        public EffectResult BasicMovement(GameContextCollection context, Vector3Int targetCoord, TileId userId)
        {
            SkillEffectArgs args = new SkillEffectArgs(context, targetCoord, userId, null);
            EffectResult result = ActivateSkill_Internal(SkillAttributeId.BasicMovement, args);
            return result;
        }

        public EffectResult AutoRecovery(GameContextCollection context, TileId userId)
        {
            Vector3Int coord = context.Characters.MemberDataContainer[userId].PlayerState.Position;
            SkillEffectArgs args = new SkillEffectArgs(context, coord, userId, null);
            EffectResult result = ActivateSkill_Internal(SkillAttributeId.AutoRecovery, args);
            return result;
        }

        EffectResult ActivateSkill_Internal(SkillAttributeId effectId, SkillEffectArgs arg)  
        {
            TileId targetId = arg.TargetId;
            EffectResult result = m_skillEffectCollection[effectId].Activate(arg);

            if (result.IsSuccessful && targetId != null)
            {
                CharacterState targetState = result.ResulContext.Characters.MemberDataContainer[targetId].PlayerState;
                CheckSkillResult(targetState);
            }
            else 
            { 
            
            }

            return result;
        }

        void CheckSkillResult(CharacterState affectedCharacter) 
        {
            if (affectedCharacter.IsHPZero || !affectedCharacter.IsActive)
            {
                EventInfo info = new EventInfo(affectedCharacter.TileId);
                OnCharacterDefeat?.Invoke(info);
            }
        }

        public static int GetSkillTargetLimit(int skillId, int skillLevel) 
        {
            SkillDataCollection data = m_skillDatabase.GetSkill(skillId);
            int level = GetValidSkillLevel(data.ValueCollection.SkillValues.Count, skillLevel);

            return data.ValueCollection.SkillValues[level].TargetLimit;
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


