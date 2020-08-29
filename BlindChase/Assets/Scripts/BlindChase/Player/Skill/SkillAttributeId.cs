using System;
using System.Collections.Generic;

namespace BlindChase
{
    [Serializable]
    public enum SkillAction
    {
        Increase,
        Decrease,
        Set,
        Move,
        Summon,
        None
    }

    [Serializable]
    public enum EffectTarget
    {
        HP,
        AP,
        SP,
        Defense,
        Speed,
        Stealth,
        CharacterPosition,
        ControllableObject,
        StaticObject,
        None
    }

    [Serializable]
    public struct SkillAttributeId : IEquatable<SkillAttributeId>
    {
        // This will be used to determine the method to be called for the skilleffect.
        public string EffectName { get; set; }

        public SkillAttributeId(string effectName)
        {
            EffectName = effectName;
        }
        public SkillAttributeId(SkillAttributeId id)
        {
            EffectName = id.EffectName;
        }
        public static bool operator ==(SkillAttributeId t1, SkillAttributeId t2)
        {
            return t1.EffectName == t2.EffectName;        
        }

        public static bool operator !=(SkillAttributeId t1, SkillAttributeId t2)
        {
            return t1.EffectName != t2.EffectName;       
        }

        public bool Equals(SkillAttributeId obj)
        {
            return this == obj;
        }

        public override bool Equals(object obj)
        {
            if (obj is SkillAttributeId Id)
            {
                return Equals(Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return
                $"{EffectName}".GetHashCode();
        }
    }


}


