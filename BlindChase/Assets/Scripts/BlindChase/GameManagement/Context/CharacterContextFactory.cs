using System.Collections.Generic;
using BlindChase.Events;

namespace BlindChase.GameManagement
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

        public void UpdateCharacterData(ObjectId id, CharacterStateContainer playerData)
        {
            if (Context == null) 
            {
                Context = new CharacterContext(new Dictionary<ObjectId, CharacterStateContainer>());
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

