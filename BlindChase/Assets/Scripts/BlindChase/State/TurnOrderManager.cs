using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.State;

namespace BlindChase
{
    public delegate void OnCharacterActivate(TileId activeFactionData);

    public class TurnOrderManager
    {
        Queue<TileId> m_waitingCharacters = new Queue<TileId>();

        public event OnCharacterActivate OnCharacterActivate = default;

        public TurnOrderManager(GameStateManager gameState, CharacterContextFactory characterContextFactory)
        {
            SetupQueue(characterContextFactory.Context);
            characterContextFactory.OnContextChanged += OnCharacterIdUpdate;
            gameState.OnTurnChange += NextCharacterTurn;
        }

        public TileId GetActiveCharacterId() 
        {
            return m_waitingCharacters.Peek();
        }

        public void NextCharacterTurn()
        {
            m_waitingCharacters.Enqueue(m_waitingCharacters.Dequeue());

            // Send message to listeners of the newly active faction.
            OnCharacterActivate?.Invoke(m_waitingCharacters.Peek());
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
            foreach(TileId id in temp) 
            {
                if (!newContext.MemberDataContainer.ContainsKey(id)) 
                {
                    temp.Remove(id);
                }

            }

            m_waitingCharacters = new Queue<TileId>(temp);

        }

    }

}


