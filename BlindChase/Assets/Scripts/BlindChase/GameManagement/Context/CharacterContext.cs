using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public class CharacterStateContainer
    {
        CharacterState m_state;

        public Transform PlayerTransform { get; private set; }

        public CharacterState PlayerState { 
            get { return m_state; } 
            set { m_state = new CharacterState(value); } }


        public CharacterStateContainer(Transform transform, CharacterState state) 
        {
            PlayerTransform = transform;
            PlayerState = state;
        }

        public CharacterStateContainer(CharacterStateContainer container)
        {
            PlayerTransform = container.PlayerTransform;
            PlayerState = new CharacterState(container.PlayerState);
        }
    }

    public struct CharacterContext : IBCContext
    {
        // The sum of all unit vitality in a faction for evaluating faction performance.
        Dictionary<string, float> m_factionPowerCollection;
        Dictionary<string, HashSet<ObjectId>> m_factionMemeberIdCollection;

        public Dictionary<ObjectId, CharacterStateContainer> MemberDataContainer { get; private set; }
        public HashSet<string> FactionIds { get { return new HashSet<string>(m_factionMemeberIdCollection.Keys); } }
        public IReadOnlyDictionary<string, float> FactionPowerValue { get { return m_factionPowerCollection; } }
        public IReadOnlyDictionary<string, HashSet<ObjectId>> FactionMemberIds { get { return m_factionMemeberIdCollection; } }


        public CharacterContext(Dictionary<ObjectId, CharacterStateContainer> factionData) 
        {
            MemberDataContainer = new Dictionary<ObjectId, CharacterStateContainer>(factionData);
            m_factionPowerCollection = new Dictionary<string, float>();
            m_factionMemeberIdCollection = new Dictionary<string, HashSet<ObjectId>>();

            foreach (KeyValuePair<ObjectId, CharacterStateContainer> container in MemberDataContainer)
            {
                UpdateFactionMemeberIds(container.Key);

                if (!container.Value.PlayerState.IsDefeated)
                {
                    AddToFactionPower(container.Key.FactionId, container.Value.PlayerState.Vitality);
                }
            }
        }

        public CharacterContext(CharacterContext context)
        {
            MemberDataContainer = new Dictionary<ObjectId, CharacterStateContainer>();
            m_factionPowerCollection = new Dictionary<string, float>();
            m_factionMemeberIdCollection = new Dictionary<string, HashSet<ObjectId>>();

            foreach (KeyValuePair<ObjectId, CharacterStateContainer> container in context.MemberDataContainer) 
            {
                MemberDataContainer.Add(container.Key, new CharacterStateContainer(container.Value));
                UpdateFactionMemeberIds(container.Key);

                if (!container.Value.PlayerState.IsDefeated)
                {
                    AddToFactionPower(container.Key.FactionId, container.Value.PlayerState.Vitality);
                }
            }
        }
        
        void AddToFactionPower(string factionId, float incrementBy) 
        {
            if (string.IsNullOrWhiteSpace(factionId)) 
            {
                return;
            }

            if (!m_factionPowerCollection.ContainsKey(factionId))
            {
                m_factionPowerCollection.Add(factionId, 0f);
            }
            m_factionPowerCollection[factionId] += incrementBy;
        }

        void UpdateFactionMemeberIds(ObjectId id) 
        {
            string factionId = id.FactionId;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return;
            }

            if (!m_factionMemeberIdCollection.ContainsKey(factionId) || m_factionMemeberIdCollection[factionId] == null) 
            {
                m_factionMemeberIdCollection.Add(factionId, new HashSet<ObjectId> { id });
            }
            else 
            {
                m_factionMemeberIdCollection[factionId].Add(id);
            }
        }

    }

}

