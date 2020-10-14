using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public class CharacterStateContainer
    {
        CharacterState m_characterState;

        public Transform PlayerTransform { get; private set; }

        public CharacterState PlayerState 
        { 
            // Give out a deep copy to prevent premature mutation.
            get { return new CharacterState(m_characterState); } 
            private set { m_characterState = value; } 
        }

        public CharacterStateContainer(Transform transform, CharacterState state) 
        {
            PlayerTransform = transform;
            PlayerState = state;
        }
    }

    public class CharacterContext : IBCContext
    {
        public Dictionary<ObjectId, CharacterStateContainer> MemberDataContainer { get; private set; }

        public CharacterContext(Dictionary<ObjectId, CharacterStateContainer> factionData) 
        {
            MemberDataContainer = factionData;
        }

        public CharacterContext(CharacterContext context) 
        {
            MemberDataContainer = context.MemberDataContainer;
        } 
        
    }

}

