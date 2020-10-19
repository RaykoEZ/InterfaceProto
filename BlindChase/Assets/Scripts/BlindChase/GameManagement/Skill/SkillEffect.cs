using System;
using System.Reflection;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public partial class SkillEffect
    {
        protected Func<SkillEffectArgs, SimulationResult> m_effectMethod { get; set; }

        public SkillEffect(SkillAttributeId id)
        {
            Type utility = typeof(SkillOperation);
            // Using reflection to store a delegate of the defined utility method.
            MethodInfo methodInfo = utility.GetMethod(id.EffectName);
            m_effectMethod = 
                (Func<SkillEffectArgs, SimulationResult>) 
                methodInfo?.CreateDelegate(typeof(Func<SkillEffectArgs, SimulationResult>));    
        }

        public virtual SimulationResult Activate(SkillEffectArgs skillValues) 
        {
            SimulationResult result = m_effectMethod(skillValues);

            return result;
        }

    }
}


