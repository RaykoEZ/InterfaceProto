using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{
    [Serializable]
    public struct CharacterData
    {
        public string Name;
        public List<int> SkillIds;
        public List<int> SkillLevels;
        public int MaxHP;
        public int MaxSP;
        public int BaseDefense;
        public float BaseSpeed;
        public CharacterClassType ClassType;
    }

    public class CharacterState 
    {
        public TileId TileId { get; set; }
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

        public CharacterState(
            TileId id,
            string charId,
            CharacterData charData,
            Vector3Int pos,
            bool isActive = true
            )
        {
            TileId = id;
            CharacterId = charId;
            Character = charData;
            Position = pos;

            CurrentHP = Character.MaxHP;
            CurrentSP = Character.MaxSP;
            CurrentDefense = Character.BaseDefense;
            CurrentSpeed = Character.BaseSpeed;
            IsActive = isActive;

            CurrentSkillCooldowns = new Dictionary<int, int>();

            foreach (int skillId in Character.SkillIds) 
            {
                CurrentSkillCooldowns[skillId] = 0;
            }
        }
    }


    [Serializable]
    public struct PlayerSaveData 
    {
        public int PlayerLevel;
        public Dictionary<TileId, CharacterData> CharacterStates;
    }


}


