using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    [Serializable]
    public struct SkillDataPair 
    {
        public int Id;
        public int Level;
    }

    [Serializable]
    public struct CharacterData
    {
        public string CharacterId;
        public string Name;
        // Key: Skill Id, Value: Skill Level
        public List<SkillDataPair> SkillLevels;
        public int MaxHP;
        public int MaxSP;
        public int BaseAttack;
        public int BaseDefense;
        public float BaseSpeed;
        public CharacterClassType ClassType;
    }

    [Serializable]
    public class CharacterState 
    {
        public ObjectId ObjectId { get; set; }
        public string CharacterId { get; set; }
        public CharacterData Character { get; set; }
        public Vector3Int Position { get; set; }
        public int CurrentHP { get; set; }
        public int CurrentSP { get; set; }
        public int CurrentAttack { get; set; }
        public int CurrentDefense { get; set; }
        public float CurrentSpeed { get; set; }

        public bool IsDefeated { get { return IsHPZero || !IsActive; } }
        public bool IsHPZero { get { return CurrentHP <= 0; } }
        public bool IsActive { get; set; }

        public Dictionary<int, int> CurrentSkillCooldowns { get; private set; }


        // A coefficient representing the character's vital state.
        // 0 - character is defeated
        // 1 - Character in perfect vital state.
        public float Vitality { 
            get 
            {
                if (IsHPZero || !IsActive) 
                {
                    return 0f;
                }

                float maxVitals = Character.MaxHP + 0.2f * Character.MaxSP;
                float currentVitals = CurrentHP + 0.2f * CurrentSP;
                return currentVitals / maxVitals; 
            } 
        }
        
        // Deep copy
        public CharacterState(CharacterState state) 
        {
            ObjectId = state.ObjectId;
            CharacterId = state.CharacterId;
            Character = state.Character;
            Position = state.Position;

            CurrentHP = state.CurrentHP;
            CurrentSP = state.CurrentSP;
            CurrentAttack = state.CurrentAttack;
            CurrentDefense = state.CurrentDefense;
            CurrentSpeed = state.CurrentSpeed;
            Character = state.Character;
            IsActive = state.IsActive;

            CurrentSkillCooldowns = new Dictionary<int, int>(state.CurrentSkillCooldowns.Count);
            foreach (KeyValuePair<int, int> skillCooldown in state.CurrentSkillCooldowns)
            {
                CurrentSkillCooldowns[skillCooldown.Key] = skillCooldown.Value;
            }
        }

        public CharacterState(
            ObjectId id,
            string charId,
            CharacterData charData,
            Vector3Int pos,
            bool isActive = true
            )
        {
            ObjectId = id;
            CharacterId = charId;
            Character = charData;
            Position = pos;

            CurrentHP = Character.MaxHP;
            CurrentSP = Character.MaxSP;
            CurrentAttack = Character.BaseAttack;
            CurrentDefense = Character.BaseDefense;
            CurrentSpeed = Character.BaseSpeed;
            IsActive = isActive;

            CurrentSkillCooldowns = new Dictionary<int, int>(Character.SkillLevels.Count);

            foreach (SkillDataPair SkillLevel in Character.SkillLevels) 
            {
                CurrentSkillCooldowns[SkillLevel.Id] = 0;
            }
        }

        public float EstimateThreat(CharacterState attacker)
        {
            float speedRatio = attacker.CurrentSpeed / CurrentSpeed;
            float damageEstimate = speedRatio * (attacker.CurrentAttack - CurrentDefense);

            return damageEstimate;
        }
    }

    [Serializable]
    public struct PlayerSaveData 
    {
        public int PlayerLevel;
        public Dictionary<ObjectId, CharacterData> CharacterStates;
    }


}


