using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.State;

namespace BlindChase
{

    public class TurnOrderManager
    {
        Queue<TileId> m_waitingCharacters = new Queue<TileId>();

        public event OnCharacterTileActivate OnCharacterTurnStart = default;

        public TurnOrderManager(GameStateManager gameState, CharacterContextFactory characterContextFactory)
        {
            SetupQueue(characterContextFactory.Context);
            characterContextFactory.OnContextChanged += OnCharacterIdUpdate;
            gameState.OnTurnStart += NextPlayerTurn;
            gameState.OnTurnEnd += OnPlayerTurnEnd;
        }

        public TileId GetActiveCharacterId() 
        {
            return m_waitingCharacters.Peek();
        }

        void NextPlayerTurn()
        {
            // Send message to listeners of the newly active faction.
            OnCharacterTurnStart?.Invoke(m_waitingCharacters.Peek());
        }

        void OnPlayerTurnEnd() 
        {
            m_waitingCharacters.Enqueue(m_waitingCharacters.Dequeue());
        }

        void SetupQueue(CharacterContext newContext) 
        {
            foreach (TileId id in newContext?.MemberDataContainer.Keys)
            {
                m_waitingCharacters.Enqueue(id);
            }
        }

        void OnCharacterIdUpdate(CharacterContext newContext) 
        {
            List<TileId> temp = new List<TileId>(m_waitingCharacters);
            List<TileId> toRemove = new List<TileId>();
            List<TileId> toAdd = new List<TileId>();

            // See which characters are removed from play
            foreach(TileId id in temp) 
            {
                if (!newContext.MemberDataContainer[id].PlayerState.IsActive) 
                {
                    toRemove.Add(id);
                }
            }

            // See if there are any to add
            foreach(TileId id in newContext.MemberDataContainer.Keys) 
            {
                if (!temp.Contains(id) && newContext.MemberDataContainer[id].PlayerState.IsActive) 
                {
                    toAdd.Add(id);
                }
            }

            // Remove characters from play
            foreach (TileId id in toRemove)
            {
                temp.Remove(id);
            }

            // Add characters into play
            foreach (TileId id in toAdd)
            {
                temp.Add(id);
            }

            m_waitingCharacters = new Queue<TileId>(temp);

        }

    }

}


