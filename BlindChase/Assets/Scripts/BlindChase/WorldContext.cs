using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase
{
    public class WorldContext 
    {
        public Tilemap WorldMap { get; private set; }

        public BCEventHandler EventHandler { get; private set; }

        public WorldContext(Tilemap map, BCEventHandler e)
        {
            WorldMap = map;
            EventHandler = e;
        }

        public WorldContext(WorldContext w) 
        {
            WorldMap = w.WorldMap;
            EventHandler = w.EventHandler;
        }
    }


    public delegate void OnWorldUpdate(WorldContext context);
    public class WorldContextFactory : IBCContextFactory
    {
        public WorldContext Context { get; private set; }
        public OnWorldUpdate OnContextChanged { get; private set; }

        public WorldContext Init(Tilemap map, BCEventHandler e) 
        {
            UpdateContext(map, e);
            return Context;
        }

        public void UpdateContext(Tilemap map, BCEventHandler e) 
        {
            Context = new WorldContext(map, e);

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

