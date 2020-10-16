using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public class CharacterStateContainer
    {
        public Transform PlayerTransform { get; private set; }

        public CharacterState PlayerState { get; private set; } 
        

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

