namespace BlindChase
{

    public class WorldStateContextFactory : IBCContextFactory<WorldStateContext>
    {
        public WorldStateContext Context { get; private set; }
        public event OnWorldStateUpdate OnContextChanged;

        public void Update(WorldStateContext newContext) 
        {
            Context = new WorldStateContext(newContext);
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new WorldStateContext(Context));
        }

        public void Shutdown() 
        {
            OnContextChanged = null;
        }
    }
}

