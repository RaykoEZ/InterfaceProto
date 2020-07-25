using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class TileManager : ITileManager
    {
        protected TileSpawner m_spawner = new TileSpawner();

        Dictionary<string, TileContainer> m_displayTiles = new Dictionary<string, TileContainer>();

        public event OnTileTrigger<TileEventInfo> OnTileEvent = default;
        public virtual void Init()
        {

        }

        public virtual void Shutdown() 
        {
            OnTileEvent = null;       
        }

        public virtual GameObject SpawnTile(string id, GameObject objectRef, Vector3 position, Transform parent, bool isActive = true)
        {

            GameObject o = m_spawner.SpawnTile(objectRef, position, parent, isActive);
            TileItem item = new TileItem { TileObject = o, Behaviour = o.GetComponent<TileBehaviour>() };


            if (m_displayTiles.ContainsKey(id) && m_displayTiles[id] != null) 
            {
                m_displayTiles[id].AddTileItem(item);
            }
            else 
            {
                TileContainer newTiles = new TileContainer(item);
                m_displayTiles.Add(id, newTiles);
            }

            return o;
        }

        protected void OnTileEventTriggered(TileEventInfo info) 
        {
            OnTileEvent?.Invoke(info);
        }

        public bool DoTilesExist(string id)
        {
            return m_displayTiles.ContainsKey(id) && m_displayTiles[id] != null;
        }

        public bool AreTilesActive(string id)
        {
            return DoTilesExist(id) && m_displayTiles[id].isActive;
        }

        void SubscribeToTileEvents(TileContainer container) 
        {
            container.OnTileTrigger += OnTileEventTriggered;
        }

        void UnsubscribeToTileEvents(TileContainer container)
        {
            container.OnTileTrigger -= OnTileEventTriggered;
        }

        public virtual void DespawnTiles(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            UnsubscribeToTileEvents(m_displayTiles[id]);
            m_displayTiles[id].DestroyTiles();
            m_displayTiles.Remove(id);
        }



        /// Tile Operations

        public virtual void ShowTile(string id)
        {
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.ShowTiles();
                SubscribeToTileEvents(container);

            }
        }

        public virtual void HideTile(string id)
        {
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.HideTiles();
                UnsubscribeToTileEvents(container);

            }
        }

        public virtual void MoveTile(string id, Vector3 offset)
        {
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.MoveTiles(offset);
            }
        }
    }

}

