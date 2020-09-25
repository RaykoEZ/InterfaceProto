using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.State;

namespace BlindChase
{

    public class TurnOrderManager : MonoBehaviour
    {
        [SerializeField] TurnOrderBar m_turnOrderBar = default;
        HashSet<TileId> m_waitingCharacters = new HashSet<TileId>();

        public event OnCharacterActivate OnCharacterTurnStart = default;
        public void Init(GameStateManager gameState, CharacterContextFactory characterContextFactory)
        {
            SetupCharacterList(characterContextFactory.Context);
            characterContextFactory.OnContextChanged += OnCharacterIdUpdate;
            gameState.OnTurnStart += NextPlayer;
            m_turnOrderBar.OnCharacterGoalReached += OnPlayerTurn;
        }

        void NextPlayer()
        {
            // Resume the turn order race
            m_turnOrderBar.StartRace();
        }

        void OnPlayerTurn(TileId id)
        {
            // Send message to listeners of the newly active faction.
            OnCharacterTurnStart?.Invoke(id);
        }

        void SetupCharacterList(CharacterContext newContext) 
        {
            foreach (TileId id in newContext?.MemberDataContainer.Keys)
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

        void AddToTurnOrderBar(List<TileId> ids, CharacterContext context)
        {
            foreach (TileId id in ids)
            {
                CharacterTurnRacer racer = new CharacterTurnRacer
                {
                    Progress = 0f,
                    Speed = context.MemberDataContainer[id].PlayerState.CurrentSpeed
                };
                m_turnOrderBar.AddParticipant(id, racer);
            }
        }

        void RemoveFromTurnOrderBar(List<TileId> ids)
        {
            foreach (TileId id in ids)
            {
                m_turnOrderBar.RemoveParticipant(id);
            }
        }

        void OnCharacterIdUpdate(CharacterContext newContext) 
        {
            List<TileId> toRemove = new List<TileId>();
            List<TileId> toAdd = new List<TileId>();

            // See which characters are removed from play
            foreach(TileId id in m_waitingCharacters) 
            {
                if (!newContext.MemberDataContainer[id].PlayerState.IsActive) 
                {
                    toRemove.Add(id);
                }
            }

            // See if there are any new active ids to add
            foreach(TileId id in newContext.MemberDataContainer.Keys) 
            {
                if (!m_waitingCharacters.Contains(id) && 
                    newContext.MemberDataContainer[id].PlayerState.IsActive) 
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


