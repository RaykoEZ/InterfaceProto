using System;

namespace BlindChase.GameManagement
{
    public class SkillActivationInput 
    {
        readonly GameContextRecord m_record;

        public readonly int SkillId;
        public readonly int SkillLevel;
        public GameContextRecord Context { get { return new GameContextRecord(m_record); } }
        public readonly ObjectId UserId;

        public SkillActivationInput(int skillId, int skillLevel, ObjectId userId, GameContextRecord context) 
        {
            SkillId = skillId;
            SkillLevel = skillLevel;
            UserId = userId;
            m_record = context;
        }

        public SkillActivationInput(in SkillActivationInput input)
        {
            SkillId = input.SkillId;
            SkillLevel = input.SkillLevel;
            UserId = input.UserId;
            m_record = input.Context;
        }

        public void OnSkillConsumption(int skillId, int cooldown, int cost) 
        {
            m_record.CharacterRecord.MemberDataContainer[UserId].PlayerState.CurrentSkillCooldowns[skillId] = cooldown;
            int currentSp = m_record.CharacterRecord.MemberDataContainer[UserId].PlayerState.CurrentSP;
            m_record.CharacterRecord.MemberDataContainer[UserId].PlayerState.CurrentSP = Math.Max(0, currentSp - cost);
        }
    }

}


