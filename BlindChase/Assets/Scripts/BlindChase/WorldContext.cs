using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase
{
    public class WorldContext : IBCContext
    {
        public Tilemap WorldMap { get; private set; }

        public WorldContext(Tilemap map)
        {
            WorldMap = map;
        }

        public WorldContext(WorldContext w) 
        {
            WorldMap = w.WorldMap;
        }
    }


    public delegate void OnWorldUpdate(WorldContext context);
    public class WorldContextFactory : IBCContextFactory<WorldContext>
    {
        public WorldContext Context { get; private set; }
        public OnWorldUpdate OnContextChanged { get; private set; }

        public void Update(WorldContext newContext) 
        {
            Context = new WorldContext(newContext);
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new WorldContext(Context));
        }

        public void SubscribeToContextUpdate(OnWorldUpdate func) 
        {
            OnContextChanged += func;
        }

        public void UnsubscribeToContextUpdate(OnWorldUpdate func)
        {
            OnContextChanged -= func;
        }

        public void Shutdown() 
        {
            OnContextChanged = null;
        }
    }
}

