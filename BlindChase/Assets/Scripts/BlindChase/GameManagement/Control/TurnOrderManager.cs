using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.State;
using BlindChase.UI;
using BlindChase.Events;

namespace BlindChase.GameManagement
{
    public class TurnOrderManager : MonoBehaviour
    {
        [SerializeField] TurnOrderBar m_turnOrderBar = default;

        HashSet<ObjectId> m_waitingCharacters = new HashSet<ObjectId>();
        public event OnCharacterActivate OnTurnStart = default;

        public void Init(TurnProgressManager gameState, CharacterContextFactory characterContextFactory)
        {
            SetupCharacterList(characterContextFactory.Context);
            characterContextFactory.OnContextChanged += OnCharacterIdUpdate;
            gameState.OnTurnStart += NextPlayer;
            m_turnOrderBar.OnCharacterGoalReached += OnPlayerTurn;
        }

        public void Shutdown()
        {
            OnTurnStart = null;
            m_turnOrderBar.OnCharacterGoalReached -= OnPlayerTurn;
        }

        void NextPlayer()
        {
            // Resume the turn order race
            m_turnOrderBar.StartRace();
        }

        void OnPlayerTurn(ObjectId id)
        {
            OnTurnStart?.Invoke(id);
        }

        void SetupCharacterList(in CharacterContext newContext) 
        {
            foreach (ObjectId id in newContext.MemberDataContainer.Keys)
            {
                CharacterTurnRacer racer = new CharacterTurnRacer
                {
                    Progress = 0f,
                    Speed = newContext.MemberDataContainer[id].PlayerState.CurrentSpeed
                };

                m_waitingCharacters.Add(id);
                m_turnOrderBar.AddParticipant(id, racer);
            }
        }

        void AddToTurnOrderBar(List<ObjectId> ids, CharacterContext context)
        {
            foreach (ObjectId id in ids)
            {
                CharacterTurnRacer racer = new CharacterTurnRacer
                {
                    Progress = 0f,
                    Speed = context.MemberDataContainer[id].PlayerState.CurrentSpeed
                };
                m_turnOrderBar.AddParticipant(id, racer);
            }
        }

        void RemoveFromTurnOrderBar(List<ObjectId> ids)
        {
            foreach (ObjectId id in ids)
            {
                m_turnOrderBar.RemoveParticipant(id);
            }
        }

        void OnCharacterIdUpdate(in CharacterContext newContext) 
        {
            List<ObjectId> toRemove = new List<ObjectId>();
            List<ObjectId> toAdd = new List<ObjectId>();

            // See which characters are removed from play
            foreach(ObjectId id in m_waitingCharacters) 
            {
                if (newContext.MemberDataContainer[id].PlayerState.IsDefeated) 
                {
                    toRemove.Add(id);
                }
            }

            // See if there are any new active ids to add
            foreach(ObjectId id in newContext.MemberDataContainer.Keys) 
            {
                if (!m_waitingCharacters.Contains(id) && 
                    !newContext.MemberDataContainer[id].PlayerState.IsDefeated) 
                {
                    toAdd.Add(id);
                }
            }

            if(toAdd.Count > 0) 
            {
                AddToTurnOrderBar(toAdd, newContext);
                // Add and new ids, then remove the inactive ones.
                m_waitingCharacters.UnionWith(toAdd);
            }

            if (toRemove.Count > 0)
            {
                RemoveFromTurnOrderBar(toRemove);
                m_waitingCharacters.ExceptWith(toRemove);
            }
        }
    }

}


