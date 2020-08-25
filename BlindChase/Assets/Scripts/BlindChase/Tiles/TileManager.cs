using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class TileManager
    {
        protected TileSpawner m_spawner = new TileSpawner();

        Dictionary<TileId, TileContainer> m_displayTiles = new Dictionary<TileId, TileContainer>();

        public event OnTileCommand<TileEventInfo> OnTileEvent = default;
        public event OnTileSelected<TileId> OnTileSelect = default;


        public virtual void Init()
        {
        }

        public virtual void Shutdown() 
        {
            OnTileEvent = null;
            OnTileSelect = null;
        }

        public virtual GameObject SpawnTile(TileId id, GameObject objectRef, Vector3 position, Transform parent, bool isActive = true)
        {
            if (id == null) 
            {
                Debug.LogError("Null ID when calling SpawnTile in TileMananger.cs");
            }
            GameObject o = m_spawner.SpawnTile(id, objectRef, position, parent, isActive);
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
            if (info == null)
            {
                return;
            }
            OnTileEvent?.Invoke(info);
        }

        protected void OnTileSelected(TileId info)
        {
            if (info == null)
            {
                return;
            }
            OnTileSelect?.Invoke(info);
        }

        protected void SubscribeToTileEvents(TileContainer container)
        {
            container.OnTileTrigger += OnTileEventTriggered;
            container.OnTileSelect += OnTileSelected;
        }

        protected void UnsubscribeToTileEvents(TileContainer container)
        {
            container.OnTileTrigger -= OnTileEventTriggered;
            container.OnTileSelect -= OnTileSelected;
        }

        public bool DoTilesExist(TileId id)
        {
            if (id == null)
            {
                return false;
            }
            return m_displayTiles.ContainsKey(id) && m_displayTiles[id] != null;
        }

        public bool AreTilesActive(TileId id)
        {
            if (id == null)
            {
                return false;
            }
            return DoTilesExist(id) && m_displayTiles[id].isActive;
        }



        public virtual void DespawnTiles(TileId id)
        {
            if (id == null)
            {
                return;
            }

            UnsubscribeToTileEvents(m_displayTiles[id]);
            m_displayTiles[id].DestroyTiles();
            m_displayTiles.Remove(id);
        }



        /// Tile Operations

        public virtual void ShowTile(TileId id)
        {
            if (id == null)
            {
                return;
            }
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.ShowTiles();
                SubscribeToTileEvents(container);

            }
        }

        public virtual void HideTile(TileId id)
        {
            if (id == null)
            {
                return;
            }
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.HideTiles();
                UnsubscribeToTileEvents(container);

            }
        }

        public virtual void MoveTile(TileId id, Vector3 offset)
        {
            if (id == null)
            {
                return;
            }
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.MoveTiles(offset);
            }
        }

    }

}

