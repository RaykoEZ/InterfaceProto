using System;
using BlindChase.State;

namespace BlindChase
{
    public abstract class SkillEffect<T> where T : GameState
    {
        protected SkillAttribute m_skillAttribute;
        public SkillTargetOption EffectTarget { get; private set; }

        // Type of game state for when this effect activates
        public Type EffectTiming { get; private set; }
        public int Delay { get; private set; }

        public SkillEffect(SkillAttribute skillAttribute, SkillTargetOption target, int delay, T timing) 
        {
            m_skillAttribute = skillAttribute;
            EffectTiming = timing.GetType();
            EffectTarget = target;
            Delay = delay;
        }

        public virtual GameEffect Activate() 
        {
            GameEffect effect = new GameEffect(new Action(EffectCall), Delay);
            return effect;
        }

        protected abstract void EffectCall();

    }
}


