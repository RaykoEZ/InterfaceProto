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
        RangeMapDatabase m_rangeMapDatabase = default;
        RangeMap m_moveRangeRef;
        NpcParameter m_defaultDetailRef;
        ObjectId m_activeNpcId;
        WorldContext m_worldRef;
        CharacterContext m_characterRef;

        public void Shutdown() 
        {
            OnNPCTaskPlanned = null;         
        }

        public virtual void Init() 
        {
            m_rangeMapDatabase = ScriptableObject.CreateInstance<RangeMapDatabase>();
            m_decsionHelper.Init(m_rangeMapDatabase);
        }

        public virtual void OnActive(ObjectId activateId, NpcParameter nature, CharacterContext c, WorldContext w) 
        {
            m_defaultDetailRef = nature;
            m_activeNpcId = activateId;
            m_characterRef = c;
            m_worldRef = w;
            CommandRequestInfo result = Plan();
            OnTaskPlanned(result);
        }

        protected virtual CommandRequestInfo Plan()
        {
            CharacterState self = m_characterRef.MemberDataContainer[m_activeNpcId].PlayerState;
            m_moveRangeRef = m_rangeMapDatabase.GetClassRangeMap(self.Character.ClassType);
            GameContextRecord record = new GameContextRecord(m_worldRef, m_characterRef);
            // Get current goal priority wieghts
            NpcParameter currentPriorities = NpcParameter.CalculateNewParameter(m_defaultDetailRef, m_moveRangeRef, record, self);

            CommandRequest decision = CommandDecision(currentPriorities, self);
            CommandRequestInfo result = new CommandRequestInfo(m_activeNpcId, decision);

            return result;
        }

        // IMPL
        CommandRequest CommandDecision(
            NpcParameter priorityDetail,
            CharacterState npcState) 
        {
            GameContextRecord context = new GameContextRecord(m_worldRef, m_characterRef);
            return m_decsionHelper.MakeDecision(priorityDetail, npcState, m_moveRangeRef, context);
        }

        void OnTaskPlanned(CommandRequestInfo info) 
        {
            OnNPCTaskPlanned?.Invoke(info);
        }
    }

}


