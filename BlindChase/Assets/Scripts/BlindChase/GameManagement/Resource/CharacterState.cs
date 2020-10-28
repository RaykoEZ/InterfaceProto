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
        public int CurrentDefense { get; set; }
        public float CurrentSpeed { get; set; }

        public bool IsHPZero { get { return CurrentHP < 0; } }

        public Dictionary<int, int> CurrentSkillCooldowns { get; private set; }

        public bool IsActive { get; set; }

        // Deep copy
        public CharacterState(CharacterState state) 
        {
            ObjectId = state.ObjectId;
            CharacterId = state.CharacterId;
            Character = state.Character;
            Position = state.Position;

            CurrentHP = state.CurrentHP;
            CurrentSP = state.CurrentSP;
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
            CurrentDefense = Character.BaseDefense;
            CurrentSpeed = Character.BaseSpeed;
            IsActive = isActive;

            CurrentSkillCooldowns = new Dictionary<int, int>(Character.SkillLevels.Count);

            foreach (SkillDataPair SkillLevel in Character.SkillLevels) 
            {
                CurrentSkillCooldowns[SkillLevel.Id] = 0;
            }
        }
    }

    [Serializable]
    public struct PlayerSaveData 
    {
        public int PlayerLevel;
        public Dictionary<ObjectId, CharacterData> CharacterStates;
    }


}


