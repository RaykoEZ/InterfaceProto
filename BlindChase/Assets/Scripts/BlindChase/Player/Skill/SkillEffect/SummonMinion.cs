using System;
using BlindChase.State;

namespace BlindChase
{
    public class SummonMinion<T> : SummonEffect<T> where T : GameState
    {
        public SummonMinion(SkillAttribute skillAttribute, SkillTargetOption target, int delay, T timing) : base(skillAttribute, target, delay, timing)
        {
        }

        public override GameEffect Activate()
        {
            GameEffect effect = new GameEffect(new Action(EffectCall), Delay);
            return effect;
        }

        protected override void EffectCall()
        {

        }
    }


}


