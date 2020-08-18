using System.Collections.Generic;
using UnityEngine;

namespace BlindChase
{
    public class ControllableDataContainer
    {
        public Vector3Int PlayerCoord { get; private set; }
        public Transform PlayerTransform { get; private set; }

        public ControllableDataContainer(Vector3Int coord, Transform transform) 
        {
            PlayerCoord = coord;
            PlayerTransform = transform;
        }
    }

    public class FactionContext : IBCContext
    {
        public Dictionary<TileId, ControllableDataContainer> MemberDataContainer { get; private set; }

        public FactionContext(Dictionary<TileId, ControllableDataContainer> factionData) 
        {
            MemberDataContainer = factionData;
        }

        public FactionContext(FactionContext context) 
        {
            MemberDataContainer = context.MemberDataContainer;
        }
    }

}

