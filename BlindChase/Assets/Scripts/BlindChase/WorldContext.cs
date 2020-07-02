using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

namespace BlindChase
{
    public class WorldContext 
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
    public class WorldContextFactory : IBCContextFactory
    {
        public WorldContext Context { get; private set; }
        public OnWorldUpdate OnContextChanged { get; private set; }

        public WorldContext Init(Tilemap map) 
        {
            UpdateContext(map);
            return Context;
        }

        public void UpdateContext(Tilemap map) 
        {
            Context = new WorldContext(map);

            OnContextUpdated();
        }

        public void OnContextUpdated()
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

