using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlindChase.GameManagement
{
    // Specify which entities are targetable by this skill.
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum AllowedTarget 
    { 
        Position = 1 << 0,
        Enemy = 1 << 1,
        Ally = 1 << 2,
        None = 0,
        Any = ~None
    }


    // Skill data for all skill levels
    [Serializable]
    public struct SkillDataCollection 
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string SkillIcon { get; set; }      
        public int MaxSkillLevel { get; set; }
        public AllowedTarget AllowedTargets { get; set; }

        // Each item defines the skill definition. 
        public SkillValueCollection ValueCollection { get; set; }
    }

    // Skill data structure defining all skill levels
    [Serializable]
    public struct SkillValueCollection 
    {
        // The operation id for this skill
        public SkillAttributeId AttributeId { get; set; }

        public List<SkillParameters> SkillValues { get; set; }
    }

    // All values of the skill for a skill level
    public class SkillParameters
    {        
        // if -1, Skill is AOE
        public int TargetLimit { get; set; }
        // Number of turns until the skill is usable again
        public int Cooldown { get; set; }
        // Skill points needed to use this skill
        public int SkillCost { get; set; }
        public string EffectRange { get; set; }

        // For each skill level...
        public int BaseValue { get; set; }
        public int KnockPower { get; set; }
        public int Delay { get; set; }
        public int Duration { get; set; }
    }
}


