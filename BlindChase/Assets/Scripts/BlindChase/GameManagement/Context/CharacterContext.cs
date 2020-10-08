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

        public List<CharacterStateContainer> GetFactionData(string faction) 
        {
            List<CharacterStateContainer> ret = new List<CharacterStateContainer>();

            foreach(ObjectId id in MemberDataContainer.Keys) 
            { 
                if (id.FactionId == faction) 
                {
                    ret.Add(MemberDataContainer[id]);
                }
            }

            return ret;
        }

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

