using System;
using System.Collections.Generic;
using BlindChase.Events;

namespace BlindChase.GameManagement
{
    public struct ActionParticipants 
    {
        public ObjectId InitiateId;
        public List<CharacterState> Affected;
    } 

    public class ActionSequenceHandler
    {
        public event OnCharacterDefeated OnCharacterDefeat = default;

        CharacterManager m_characterManagerRef;
        ActionParticipants m_currentParticipants;
        Action m_onFinish;
        int m_actionsCompleted = 0;

        public void Init(CharacterManager characterManager) 
        {
            m_characterManagerRef = characterManager;
        }

        public void OnCharacterAdvance(ActionParticipants participants, Action onFinish, Dictionary<string, object> payload = null)
        {
            SetupSequence(participants, onFinish);
            // Add a trigger for calling takedamage when attack animation calls for an OnHit event
            List<Action<bool>> onHitTriggers = new List<Action<bool>>();
            CharacterState attacker = default;

            foreach (CharacterState participant in participants.Affected) 
            { 
                if(participant.ObjectId == participants.InitiateId) 
                {
                    attacker = participant;
                }
                else 
                {
                    Action<bool> onHit = (isLastHit) => 
                    {
                        m_characterManagerRef.OnCharacterTakeDamage(participant.ObjectId, participant.Position, OnActionComplete, isLastHit);

                    };
                    onHitTriggers.Add(onHit);                
                }
            }

            m_characterManagerRef.OnCharacterAdvance(
                attacker.ObjectId, 
                attacker.Position, 
                OnActionComplete, 
                onHit: onHitTriggers.Count > 0? onHitTriggers : null);
        }

        public void OnCharacterSkill(ActionParticipants participants, Action onFinish, Dictionary<string, object> payload = null)
        {
            SetupSequence(participants, onFinish);

            m_characterManagerRef.OnCharacterSkillActivate(participants.InitiateId);

            string skillId = payload["SkillId"].ToString();
            if(skillId == null) 
            {
                return;
            }
            // Get skill animation and trigger user skill animation
            // Add a trigger for calling takedamage when attack animation calls for an OnHit event

            // IMPL
            foreach (CharacterState participant in participants.Affected)
            {

            }
            OnActionComplete();
            OnActionComplete();
        }

        void SetupSequence(ActionParticipants participants, Action onFinish) 
        {
            m_currentParticipants = participants;
            m_onFinish = onFinish;
        }

        void OnActionComplete() 
        {
            ++m_actionsCompleted;
            if (m_actionsCompleted >= m_currentParticipants.Affected.Count) 
            {
                HandleDefeated();

                m_actionsCompleted = 0;
                m_onFinish?.Invoke();
            }
        }

        void HandleDefeated()
        {
            foreach (CharacterState character in m_currentParticipants.Affected)
            {
                if (!character.IsDefeated)
                {
                    // Player defeated anim here

                    // Send event for defeat
                    EventInfo info = new EventInfo(character.ObjectId);
                    OnCharacterDefeat?.Invoke(info);
                }
            }
        }
    }

}


