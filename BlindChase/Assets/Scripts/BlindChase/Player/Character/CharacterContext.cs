using System.Collections.Generic;
using UnityEngine;

namespace BlindChase
{
    public class ControllableDataContainer
    {
        public Transform PlayerTransform { get; private set; }

        public CharacterState PlayerState { get; private set; }

        public ControllableDataContainer(Transform transform, CharacterState state) 
        {
            PlayerTransform = transform;
            PlayerState = state;
        }
    }

    public class CharacterContext : IBCContext
    {
        public Dictionary<TileId, ControllableDataContainer> MemberDataContainer { get; private set; }

        public CharacterContext(Dictionary<TileId, ControllableDataContainer> factionData) 
        {
            MemberDataContainer = factionData;
        }

        public CharacterContext(CharacterContext context) 
        {
            MemberDataContainer = context.MemberDataContainer;
        }
    }

}

