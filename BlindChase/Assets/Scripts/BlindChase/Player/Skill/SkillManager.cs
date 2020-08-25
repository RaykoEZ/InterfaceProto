using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase
{
    public class SkillManager
    {
        static SkillDatabase m_skillDatabase = default;

        public SkillManager() 
        {
            m_skillDatabase = ScriptableObject.CreateInstance<SkillDatabase>();
            m_skillDatabase.GetSkillDatabase();
        
        }

        public void ProcessSkillEffect<T>(T skill) where T : Skill 
        { 
        
        }

        public SkillData GetSkill(string id) 
        {
            return m_skillDatabase?.GetSkill(id);
        }

    }

}


