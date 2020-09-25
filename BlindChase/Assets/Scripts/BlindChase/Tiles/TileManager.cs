using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class TileManager : MonoBehaviour
    {
        protected TileSpawner m_spawner = new TileSpawner();

        Dictionary<TileId, TileContainer> m_displayTiles = new Dictionary<TileId, TileContainer>();

        public event OnPlayerCommand<CommandEventInfo> OnTileCommand = default;

        public virtual void Init(TurnOrderManager turnOrderManager)
        {
        }

        public virtual void Shutdown() 
        {
            OnTileCommand = null;
        }

        public virtual GameObject SpawnTile(
            TileId id, GameObject objectRef, Vector3 position, Transform parent, 
            CharacterData charData = default,
            PromptHandler rangeDisplay = null, 
            bool isActive = true
            )
        {
            if (id == null) 
            {
                Debug.LogError("Null ID when calling SpawnTile in TileMananger.cs");
            }
            GameObject o = m_spawner.SpawnTile(id, objectRef, position, parent, charData, isActive);
            TileItem item = new TileItem { TileObject = o, Behaviour = o.GetComponent<TileBehaviour>() };

            if (m_displayTiles.ContainsKey(id) && m_displayTiles[id] != null) 
            {
                m_displayTiles[id].AddTileItem(item);
            }
            else 
            {
                TileContainer newTiles = new TileContainer();
                newTiles.AddTileItem(item);
                m_displayTiles.Add(id, newTiles);
            }
            return o;
        }

        protected virtual void OnTileEventTriggered(CommandEventInfo info) 
        {
            if (info == null)
            {
                return;
            }
            OnTileCommand?.Invoke(info);
        }

        protected virtual void SubscribeToCommand(TileContainer container)
        {
            container.OnTileTrigger += OnTileEventTriggered;
        }

        protected virtual void UnsubscribeToCommand(TileContainer container)
        {
            container.OnTileTrigger -= OnTileEventTriggered;
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

            UnsubscribeToCommand(m_displayTiles[id]);
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
                SubscribeToCommand(container);

            }
        }

        public virtual void HideTile(TileId id)
        {
            if (id == null || !m_displayTiles.ContainsKey(id))
            {
                return;
            }
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.HideTiles();
                UnsubscribeToCommand(container);

            }
        }

        public virtual void HideAll()
        {
            foreach (TileId id in m_displayTiles.Keys) 
            {
                HideTile(id);
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

