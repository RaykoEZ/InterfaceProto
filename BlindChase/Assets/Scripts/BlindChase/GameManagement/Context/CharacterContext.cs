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
        public Dictionary<ObjectId, CharacterStateContainer> MemberDataContainer { get; private set; }

        public CharacterContext(Dictionary<ObjectId, CharacterStateContainer> factionData) 
        {
            MemberDataContainer = factionData;
        }

        public CharacterContext(CharacterContext context)
        {
            MemberDataContainer = new Dictionary<ObjectId, CharacterStateContainer>();
            foreach (KeyValuePair<ObjectId, CharacterStateContainer> container in context.MemberDataContainer) 
            {
                MemberDataContainer.Add(container.Key, new CharacterStateContainer(container.Value));
            }
            
        } 
        
    }

}

