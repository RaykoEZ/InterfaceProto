using BlindChase.Events;

namespace BlindChase.GameManagement
{
    public class WorldStateContextFactory : IBCContextFactory<WorldContext>
    {
        public WorldContext Context { get; private set; }
        public event OnWorldStateUpdate OnContextChanged;

        public void Update(WorldContext newContext) 
        {
            Context = new WorldContext(newContext);
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new WorldContext(Context));
        }

        public void Shutdown() 
        {
            OnContextChanged = null;
        }
    }
}

