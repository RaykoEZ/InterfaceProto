using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BlindChase.State;

namespace BlindChase
{
    public class SkillEffectArgs 
    {
        public GameContextCollection Context { get; private set; }
        public HashSet<TileId> TargetIds { get; private set; }
        public TileId UserId { get; private set; }
        public SkillDataItem SkillData { get; private set; }
        
        public SkillEffectArgs(GameContextCollection context, HashSet<TileId> target, TileId user, SkillDataItem args) 
        {
            Context = context;
            TargetIds = target;
            UserId = user;
            SkillData = args;
        }
    }

    public partial class SkillEffect
    {
        protected Func<SkillEffectArgs, GameContextCollection> m_effectMethod { get; set; }

        public SkillEffect(SkillAttributeId id)
        {
            Type utility = typeof(SkillOperation);
            // Using reflection to store a delegate of the defined utility method.
            MethodInfo methodInfo = utility.GetMethod(id.EffectName);
            m_effectMethod = 
                (Func<SkillEffectArgs, GameContextCollection>) 
                methodInfo?.CreateDelegate(typeof(Func<SkillEffectArgs, GameContextCollection>));    
        }

        public virtual GameContextCollection Activate(SkillEffectArgs skillValues) 
        {
            GameContextCollection result = m_effectMethod(skillValues);

            return result;
        }

    }
}


