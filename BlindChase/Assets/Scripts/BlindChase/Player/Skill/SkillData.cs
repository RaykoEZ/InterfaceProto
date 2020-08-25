using System;
using System.Collections.Generic;

namespace BlindChase
{
    [Serializable]
    public class SkillData 
    { 
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Text { get; private set; }
        // if -1, Skill is AOE
        public int TargetLimit { get; private set; }
        // Which tiles can this skill target
        public TargetOptionLimit TargetOption { get; private set; }
        // Number of turns until the skill is usable again
        public int Cooldown { get; private set; }
        // Skill points needed to use this skill
        public int SkillCost { get; private set; }

        public List<SkillAttribute> SkillAttributes { get; private set; }

        
        public SkillData(
            string id, 
            string name, 
            string text,
            int targetLimit,
            int cooldown,
            int skillCost,
            List<SkillAttribute> skillAttributes
            )
        {
            Id = id;
            Name = name;
            Text = text;
            TargetLimit = targetLimit;
            Cooldown = cooldown;
            SkillAttributes = skillAttributes;
            SkillCost = skillCost;
        }
    }

}


