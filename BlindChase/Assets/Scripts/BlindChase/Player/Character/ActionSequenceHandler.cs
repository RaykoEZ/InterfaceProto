using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{
    public struct ActionParticipants 
    {
        public TileId InitiateId;
        public List<CharacterState> Affected;
    } 

    public class ActionSequenceHandler
    {
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
                if(participant.TileId == participants.InitiateId) 
                {
                    attacker = participant;
                }
                else 
                {
                    Action<bool> onHit = (isLastHit) => 
                    {
                        m_characterManagerRef.OnCharacterTakeDamage(participant.TileId, participant.Position, OnActionComplete, isLastHit);

                    };
                    onHitTriggers.Add(onHit);                
                }
            }

            m_characterManagerRef.OnCharacterAdvance(
                attacker?.TileId, 
                attacker.Position, 
                OnActionComplete, 
                onHit: onHitTriggers.Count > 0? onHitTriggers : null);
        }

        public void OnCharacterSkill(ActionParticipants participants, Action onFinish, Dictionary<string, object> payload = null)
        {
            SetupSequence(participants, onFinish);

            string skillId = payload["SkillId"].ToString();
            if(skillId == null) 
            {
                return;
            }
            // Get skill animation and trigger user skill animation
            // Add a trigger for calling takedamage when attack animation calls for an OnHit event

            foreach (CharacterState participant in participants.Affected)
            {
            }

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
                m_actionsCompleted = 0;
                m_onFinish?.Invoke();
            }
        }

        void OnHit() 
        { 
        
        }

        public void OnCharacterDefeated(TileId id) 
        { 
        
        }
    }

}


