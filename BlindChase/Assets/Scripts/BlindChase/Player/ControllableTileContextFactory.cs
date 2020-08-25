using System.Collections.Generic;

namespace BlindChase
{
    public delegate void OnControllableUpdate(CharacterContext newContext);
    public class ControllableTileContextFactory : IBCContextFactory<CharacterContext>
    {
        public CharacterContext Context { get; private set; }
        public event OnControllableUpdate OnContextChanged = default;

        public void Update(CharacterContext newContext)
        {
            Context = new CharacterContext(newContext);
            OnContextUpdated();
        }

        public void UpdateContext(TileId id, ControllableDataContainer playerData)
        {
            if (Context == null) 
            {
                Context = new CharacterContext(new Dictionary<TileId, ControllableDataContainer>());
            }

            Context.MemberDataContainer[id] = playerData;
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new CharacterContext(Context));
        }

        public void SubscribeToContextUpdate(OnControllableUpdate func)
        {
            OnContextChanged += func;
        }

        public void UnsubscribeToContextUpdate(OnControllableUpdate func)
        {
            OnContextChanged -= func;
        }

        public void Shutdown()
        {
            OnContextChanged = null;
        }
    }
}

