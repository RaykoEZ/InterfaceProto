using System;
using System.Reflection;

namespace BlindChase.GameManagement
{
    public partial class SkillEffect
    {
        protected Func<SkillEffectArgs, SimulationResult> m_effectMethod { get; set; }

        public SkillEffect(SkillAttributeId id)
        {
            Type operationUtil = typeof(SkillOperation);
            // Using reflection to store a delegate of the defined utility method.
            MethodInfo methodInfo = operationUtil.GetMethod(id.EffectName);
            m_effectMethod = 
                (Func<SkillEffectArgs, SimulationResult>) 
                methodInfo?.CreateDelegate(typeof(Func<SkillEffectArgs, SimulationResult>));
        }

        public virtual bool IsTargetValid(in SkillEffectArgs skillValues) 
        {
            return false;
        }

        public virtual SimulationResult Activate(in SkillEffectArgs skillValues) 
        {
            SimulationResult result = m_effectMethod(skillValues);

            return result;
        }

    }
}


