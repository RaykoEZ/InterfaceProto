using System.Collections.Generic;

namespace BlindChase
{
    public delegate void OnCharacterUpdate(CharacterContext newContext);
    public class CharacterContextFactory : IBCContextFactory<CharacterContext>
    {
        public CharacterContext Context { get; private set; }
        public event OnCharacterUpdate OnContextChanged = default;

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

        public void SubscribeToContextUpdate(OnCharacterUpdate func)
        {
            OnContextChanged += func;
        }

        public void UnsubscribeToContextUpdate(OnCharacterUpdate func)
        {
            OnContextChanged -= func;
        }

        public void Shutdown()
        {
            OnContextChanged = null;
        }
    }
}

