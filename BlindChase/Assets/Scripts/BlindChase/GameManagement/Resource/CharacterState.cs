using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    [Serializable]
    public struct IdLevelPair 
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
        public List<IdLevelPair> SkillLevels;
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

            CurrentHP = state.Character.MaxHP;
            CurrentSP = state.Character.MaxSP;
            CurrentDefense = state.Character.BaseDefense;
            CurrentSpeed = state.Character.BaseSpeed;
            IsActive = state.IsActive;

            CurrentSkillCooldowns = state.CurrentSkillCooldowns;
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

            CurrentSkillCooldowns = new Dictionary<int, int>();

            foreach (IdLevelPair SkillLevel in Character.SkillLevels) 
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


