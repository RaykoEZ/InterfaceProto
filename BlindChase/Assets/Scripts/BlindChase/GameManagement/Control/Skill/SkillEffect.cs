using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public struct EffectResult
    {
        public bool IsSuccessful;
        public string Message;
        public GameContextCollection ResulContext;
        public List<CharacterState> AffectedCharacters;

        public void OnSuccess(string message, GameContextCollection result, List<CharacterState> affected) 
        {
            IsSuccessful = true;
            Message = message;
            ResulContext = result;
            AffectedCharacters = affected;
        }

        public void OnFail(string message, GameContextCollection result)
        {
            IsSuccessful = false;
            Message = message;
            ResulContext = result;
            AffectedCharacters = new List<CharacterState>();
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


