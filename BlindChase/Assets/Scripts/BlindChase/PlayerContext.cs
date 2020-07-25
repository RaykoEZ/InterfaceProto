using System.Collections.Generic;
using UnityEngine;
using BlindChase.Utility;

namespace BlindChase
{
    public class PlayerContext : IBCContext
    {
        public Vector3Int PlayerCoord { get; private set; }
        public Transform PlayerTransform { get; private set; }
        // THIS SHOULD USE A COMMAND DICTIONARY NOW
        public List<NeighbourhoodRangeMap> RangeMaps { get; private set; } = new List<NeighbourhoodRangeMap>();

        public PlayerContext(Vector3Int coord, Transform transform) 
        {
            PlayerCoord = coord;
            PlayerTransform = transform;
            RangeMaps.Add(NeighbourhoodUtil.GetNeighbourRangeMap(0));
            RangeMaps.Add(NeighbourhoodUtil.GetNeighbourRangeMap(1));
        }

        public PlayerContext(PlayerContext context) 
        {
            PlayerCoord = context.PlayerCoord;
            PlayerTransform = context.PlayerTransform;
            RangeMaps.Add(NeighbourhoodUtil.GetNeighbourRangeMap(0));
            RangeMaps.Add(NeighbourhoodUtil.GetNeighbourRangeMap(1));
        }
    }

    public delegate void OnPlayerUpdate(PlayerContext context);

    public class PlayerContextFactory : IBCContextFactory<PlayerContext>
    {
        public PlayerContext Context { get; private set; }
        public OnPlayerUpdate OnContextChanged { get; private set; }


        public void Update(PlayerContext newContext)
        {
            Context = new PlayerContext(newContext);
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new PlayerContext(Context));
        }

        public void SubscribeToContextUpdate(OnPlayerUpdate func)
        {
            OnContextChanged += func;
        }

        public void UnsubscribeToContextUpdate(OnPlayerUpdate func)
        {
            OnContextChanged -= func;
        }

        public void Shutdown()
        {
            OnContextChanged = null;
        }
    }




}

