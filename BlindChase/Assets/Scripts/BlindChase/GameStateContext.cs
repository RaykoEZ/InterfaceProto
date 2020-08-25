using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase
{
    public class GameStateContext : IBCContext
    {
        public Tilemap WorldMap { get; private set; }

        public GameStateContext(Tilemap map)
        {
            WorldMap = map;
        }

        public GameStateContext(GameStateContext w) 
        {
            WorldMap = w.WorldMap;
        }
    }


    public delegate void OnGameStateUpdate(GameStateContext context);
    public class GameStateContextFactory : IBCContextFactory<GameStateContext>
    {
        public GameStateContext Context { get; private set; }
        public OnGameStateUpdate OnContextChanged { get; private set; }

        public void Update(GameStateContext newContext) 
        {
            Context = new GameStateContext(newContext);
            OnContextUpdated();
        }

        void OnContextUpdated()
        {
            OnContextChanged?.Invoke(new GameStateContext(Context));
        }

        public void SubscribeToContextUpdate(OnGameStateUpdate func) 
        {
            OnContextChanged += func;
        }

        public void UnsubscribeToContextUpdate(OnGameStateUpdate func)
        {
            OnContextChanged -= func;
        }

        public void Shutdown() 
        {
            OnContextChanged = null;
        }
    }
}

