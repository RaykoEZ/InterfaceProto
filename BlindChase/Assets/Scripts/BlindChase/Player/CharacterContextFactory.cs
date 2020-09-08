using System.Collections.Generic;

namespace BlindChase
{
    public class CharacterContextFactory : IBCContextFactory<CharacterContext>
    {
        public CharacterContext Context { get; private set; }
        public event OnCharacterUpdate OnContextChanged = default;

        public void Update(CharacterContext newContext)
        {
            Context = new CharacterContext(newContext);
            OnContextUpdated();
        }

        public void UpdateCharacterData(TileId id, CharacterStateContainer playerData)
        {
            if (Context == null) 
            {
                Context = new CharacterContext(new Dictionary<TileId, CharacterStateContainer>());
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

