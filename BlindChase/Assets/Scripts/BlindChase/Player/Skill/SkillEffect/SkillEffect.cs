using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BlindChase.State;

namespace BlindChase
{
    public struct EffectResult
    {
        public bool IsSuccessful;
        public string Message;
        public GameContextCollection ResultStates;

        public void OnSuccess(string message, GameContextCollection result) 
        {
            IsSuccessful = true;
            Message = message;
            ResultStates = result;   
        }

        public void OnFail(string message, GameContextCollection result)
        {
            IsSuccessful = false;
            Message = message;
            ResultStates = result;
        }
    }

    public partial class SkillEffect
    {
        protected Func<SkillEffectArgs, EffectResult> m_effectMethod { get; set; }

        public SkillEffect(SkillAttributeId id)
        {
            Type utility = typeof(SkillOperation);
            // Using reflection to store a delegate of the defined utility method.
            MethodInfo methodInfo = utility.GetMethod(id.EffectName);
            m_effectMethod = 
                (Func<SkillEffectArgs, EffectResult>) 
                methodInfo?.CreateDelegate(typeof(Func<SkillEffectArgs, EffectResult>));    
        }

        public virtual EffectResult Activate(SkillEffectArgs skillValues) 
        {
            EffectResult result = m_effectMethod(skillValues);

            return result;
        }

    }
}


