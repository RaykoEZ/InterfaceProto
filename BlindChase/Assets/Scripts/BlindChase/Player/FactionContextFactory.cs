using System.Collections.Generic;

namespace BlindChase
{
    public delegate void OnFactionUpdate(FactionContext newContext);
    public class FactionContextFactory : IBCContextFactory<FactionContext>
    {
        public string FactionId { get; private set; }
        public FactionContext Context { get; private set; }
        public event OnFactionUpdate OnContextChanged = default;

        public FactionContextFactory(string factionId) 
        {
            FactionId = factionId;
        }

        public void Reset(FactionContext newContext)
        {
            Context = new FactionContext(newContext);
            OnContextUpdated();
        }

        public void UpdateContext(TileId id, ControllableDataContainer playerData)
        {
            if (Context == null) 
            {
                Context = new FactionContext(new Dictionary<TileId, ControllableDataContainer>());
            }

            Context.MemberDataContainer[id] = playerData;
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new FactionContext(Context));
        }

        public void SubscribeToContextUpdate(OnFactionUpdate func)
        {
            OnContextChanged += func;
        }

        public void UnsubscribeToContextUpdate(OnFactionUpdate func)
        {
            OnContextChanged -= func;
        }

        public void Shutdown()
        {
            OnContextChanged = null;
        }
    }
}

