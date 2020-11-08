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
        CommandSimulator m_simulatior;
        RangeMapDatabase m_rangeDatabaseRef;
        RangeOffsetMap m_visionRangeRef;
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
            m_rangeDatabaseRef = rangeDatatbaseRef;
            m_simulatior = new CommandSimulator(rangeDatatbaseRef);
        }

        public virtual void OnActive(ObjectId activateId, DecisionParameter nature, GameContextRecord context) 
        {
            m_defaultDetailRef = nature;
            m_activeNpcId = activateId;
            m_characterRef = context.CharacterRecord;
            m_worldRef = context.WorldRecord;

            CharacterState self = new CharacterState(m_characterRef.MemberDataContainer[m_activeNpcId].PlayerState);
            m_visionRangeRef = m_rangeDatabaseRef.GetSquareRadiusMap(2);
            m_simulatior.Setup(self, context);
            GameContextRecord record = new GameContextRecord(m_worldRef, m_characterRef);
            CommandRequestInfo result = Plan(record, self);

            OnTaskPlanned(result);
        }

        protected virtual CommandRequestInfo Plan(GameContextRecord record, CharacterState self)
        {
            ObservationResult observation = ObservationResult.CharactersInRange(record, m_rangeDatabaseRef, self, m_visionRangeRef);
            // Get current goal priority wieghts
            DecisionParameter currentPriorities = DecisionParameter.CalculateNewParameter(record, self, m_defaultDetailRef, observation);
            CommandRequest decision = CommandDecision(currentPriorities, record);
            CommandRequestInfo result = new CommandRequestInfo(m_activeNpcId, decision);

            return result;
        }

        CommandRequest CommandDecision(
            DecisionParameter priorityDetail,
            GameContextRecord record) 
        {
            CommandRequest desicion = m_simulatior.Simulate(priorityDetail, record);
            return desicion;
        }

        void OnTaskPlanned(CommandRequestInfo info) 
        {
            OnNPCTaskPlanned?.Invoke(info);
        }
    }

}


