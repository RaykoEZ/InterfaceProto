using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BlindChase.State;

namespace BlindChase
{
    public class SkillEffectArgs 
    {
        public Vector3Int TargetPos { get; private set; }
        public Vector3Int UserPos { get; private set; }
        public SkillDataItem SkillData { get; private set; }
        
        public SkillEffectArgs(Vector3Int target, Vector3Int user, SkillDataItem args) 
        {
            TargetPos = target;
            UserPos = user;
            SkillData = args;
        }
    }

    public partial class SkillEffect
    {
        protected Action<SkillEffectArgs> m_effectMethod { get; set; }

        public SkillEffect(SkillAttributeId id)
        {
            Type utility = typeof(SkillCollection);
            // Using reflection to store a delegate of the defined utility method.
            MethodInfo methodInfo = utility.GetMethod(id.EffectName);
            m_effectMethod = 
                (Action<SkillEffectArgs>) 
                methodInfo?.CreateDelegate(typeof(Action<SkillEffectArgs>));    
        }

        public virtual void Activate(SkillEffectArgs skillValues) 
        {
            Debug.Log("Skill Activated");
            m_effectMethod?.Invoke(skillValues);
            
        }

    }
}


