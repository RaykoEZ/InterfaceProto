using System.Collections.Generic;
using UnityEngine;

namespace BlindChase
{
    public class ControllableTileContextData
    {
        public Vector3Int PlayerCoord { get; private set; }
        public Transform PlayerTransform { get; private set; }

        public ControllableTileContextData(Vector3Int coord, Transform transform) 
        {
            PlayerCoord = coord;
            PlayerTransform = transform;
        }
    }

    public class ControllableTileContext : IBCContext
    {
        public Dictionary<TileId, ControllableTileContextData> FactionMembers { get; private set; }

        public TileId LatestSelectedPieceId { get; private set; }

        public ControllableTileContext(Dictionary<TileId, ControllableTileContextData> playerContext) 
        {
            FactionMembers = playerContext;
        }

        public ControllableTileContext(ControllableTileContext context) 
        {
            FactionMembers = context.FactionMembers;
            LatestSelectedPieceId = context.LatestSelectedPieceId;
        }

        public void SetActivePieceId(TileId tileId) 
        {
            LatestSelectedPieceId = tileId;
        }
    }

    public delegate void OnPlayerUpdate(ControllableTileContext context);

    public class FactionContextFactory : IBCContextFactory<ControllableTileContext>
    {
        public ControllableTileContext Context { get; private set; }
        public event OnPlayerUpdate OnContextChanged = default;

        public void Reset(ControllableTileContext newContext)
        {
            Context = new ControllableTileContext(newContext);
            OnContextUpdated();
        }

        public void UpdateContext(TileId id, ControllableTileContextData playerData) 
        {
            if (Context == null) 
            {
                Context = new ControllableTileContext(new Dictionary<TileId, ControllableTileContextData>());
            }

            Context.FactionMembers[id] = playerData;
        }

        public void FactionMemberSelected(TileId tileId) 
        {
            if (tileId == null || tileId == Context.LatestSelectedPieceId || !Context.FactionMembers.ContainsKey(tileId)) 
            {
                return;
            }

            Context.SetActivePieceId(tileId);
            OnContextUpdated();
        }


        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new ControllableTileContext(Context));
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

