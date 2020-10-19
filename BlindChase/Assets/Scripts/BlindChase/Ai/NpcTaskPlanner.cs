using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public class NpcTaskPlanner
    {
        public event OnPlayerCommand<CommandRequestInfo> OnNPCTaskPlanned = default;
        DecisionHelper m_decsionHelper = new DecisionHelper();
        RangeMapDatabase m_rangeMapDatabaseRef = default;
        RangeMap m_moveRangeRef;
        RangeMap m_visionRangeRef;
        DecisionParameter m_defaultDetailRef;
        ObjectId m_activeNpcId;
        WorldContext m_worldRef;
        CharacterContext m_characterRef;

        public void Shutdown() 
        {
            OnNPCTaskPlanned = null;         
        }

        public virtual void Init(RangeMapDatabase rangeDatatbaseRef) 
        {
            m_rangeMapDatabaseRef = rangeDatatbaseRef;
        }

        public virtual void OnActive(ObjectId activateId, DecisionParameter nature, CharacterContext c, WorldContext w) 
        {
            m_defaultDetailRef = nature;
            m_activeNpcId = activateId;
            m_characterRef = c;
            m_worldRef = w;

            CharacterState self = m_characterRef.MemberDataContainer[m_activeNpcId].PlayerState;
            Dictionary<int, RangeMap> skillRanges = new Dictionary<int, RangeMap>();
            foreach (IdLevelPair skillLevel in self.Character.SkillLevels) 
            {
                SkillParameters skillParam = SkillManager.GetSkillParameters(skillLevel.Id, skillLevel.Level);
                string rangeId = skillParam.EffectRange;
                skillRanges[skillLevel.Id] = m_rangeMapDatabaseRef.GetSkillRangeMap(rangeId);
            }

            m_visionRangeRef = m_rangeMapDatabaseRef.GetSquareRadiusMap(2);

            m_moveRangeRef = m_rangeMapDatabaseRef.GetClassRangeMap(self.Character.ClassType);
            GameContextRecord record = new GameContextRecord(m_worldRef, m_characterRef);
            m_decsionHelper.Setup(m_moveRangeRef, m_visionRangeRef, skillRanges, self, record);
            CommandRequestInfo result = Plan(self, record);

            OnTaskPlanned(result);
        }

        protected virtual CommandRequestInfo Plan(CharacterState self, GameContextRecord record)
        {
            // Get current goal priority wieghts
            DecisionParameter currentPriorities = DecisionParameter.CalculateNewParameter(m_defaultDetailRef, m_visionRangeRef,m_moveRangeRef, record, self);
            CommandRequest decision = CommandDecision(currentPriorities);

            CommandRequestInfo result = new CommandRequestInfo(m_activeNpcId, decision);

            return result;
        }

        // IMPL
        CommandRequest CommandDecision(
            DecisionParameter priorityDetail) 
        {
            CommandRequest desicion = m_decsionHelper.MakeDecision(priorityDetail);
            return desicion;
        }

        void OnTaskPlanned(CommandRequestInfo info) 
        {
            OnNPCTaskPlanned?.Invoke(info);
        }
    }

}


