using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public class DelayedEffect
    {
        public SkillEffect Effect { get; private set; }

        public SkillEffectArgs Args { get; private set; }
        public int Delay { get; set; }

        public DelayedEffect(SkillEffect effect, SkillEffectArgs args, int delay = 0)
        {
            Effect = effect;
            Args = args;
            Delay = delay;
        }
    }

    public class DelayedEffectHandler
    {
        Stack<DelayedEffect> m_onTurnStartEffects { get; set; } = new Stack<DelayedEffect>();
        Stack<DelayedEffect> m_onTurnEndEffects { get; set; } = new Stack<DelayedEffect>();

        public delegate void OnEffectActivated(SimulationResult effectResult);

        public event OnEffectActivated OnDelayedEffectActivate = default;

        public void OnTurnStartEffects() 
        {
            m_onTurnStartEffects = ProcessEffectStack(m_onTurnStartEffects);
        }

        public void OnTurnEndEffects() 
        {
            m_onTurnEndEffects = ProcessEffectStack(m_onTurnEndEffects);
        }

        protected Stack<DelayedEffect> ProcessEffectStack(Stack<DelayedEffect> effectStack)
        {
            Stack<DelayedEffect> delayedEffects = new Stack<DelayedEffect>();

            if (effectStack == null || effectStack.Count == 0)
            {
                return delayedEffects;
            }

            // Call all effects in this effect stack.
            foreach (DelayedEffect delayedEffect in effectStack)
            {
                delayedEffect.Delay--;

                if (delayedEffect.Delay < 0)
                {
                    SimulationResult result = delayedEffect.Effect.Activate(delayedEffect.Args);
                    OnDelayedEffectActivate?.Invoke(result);
                    effectStack.Pop();
                }
                else
                {
                    delayedEffects.Push(effectStack.Pop());
                }
            }

            return delayedEffects;
        }
    }

}


