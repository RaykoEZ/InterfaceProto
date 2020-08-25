using System;

namespace BlindChase
{
    [Serializable]
    public enum SkillAction
    {
        Lose,
        Gain,
        Set,
        Move,
        Summon,
        None
    }

    [Serializable]
    public enum EffectTarget
    {
        LifePoint,
        ActionPoint,
        SkillPoint,
        Defense,
        Speed,
        CharacterPosition,
        ControllableObject,
        StaticObject,
        None
    }

    [Flags]
    [Serializable]
    public enum TargetOptionLimit
    {
        TargetSelf = 1 << 0,
        TargetAlly = 1 << 1,
        TargetEnemy = 1 << 2,
        TargetPosition = 1 << 3
    }

    [Serializable]
    public struct SkillAttribute 
    {
        public SkillAction EffectAction { get; private set; }
        public EffectTarget EffectTarget { get; private set; }
        public int Delay { get; private set; }
        // Number of turns, -1 is permanent
        public int Duration { get; private set; }

    }


}


