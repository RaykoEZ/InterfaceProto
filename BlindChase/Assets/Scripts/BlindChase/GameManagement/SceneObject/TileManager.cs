using System.Collections.Generic;
using UnityEngine;
using BlindChase.UI;

namespace BlindChase.GameManagement 
{
    public class TileManager : MonoBehaviour
    {
        protected TileSpawner m_spawner = new TileSpawner();

        Dictionary<ObjectId, TileContainer> m_displayTiles = new Dictionary<ObjectId, TileContainer>();

        public virtual void Init()
        {
        }

        public virtual void Shutdown() 
        {
        }

        public virtual GameObject SpawnTile(
            ObjectId id, GameObject objectRef, Vector3 position, Transform parent, 
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

        public bool DoTilesExist(ObjectId id)
        {
            if (id == null)
            {
                return false;
            }
            return m_displayTiles.ContainsKey(id) && m_displayTiles[id] != null;
        }

        public bool AreTilesActive(ObjectId id)
        {
            if (id == null)
            {
                return false;
            }
            return DoTilesExist(id) && m_displayTiles[id].isActive;
        }


        public virtual void DespawnTiles(ObjectId id)
        {
            if (id == null)
            {
                return;
            }
            m_displayTiles[id].DestroyTiles();
            m_displayTiles.Remove(id);
        }



        /// Tile Operations

        public virtual void ShowTile(ObjectId id)
        {
            if (id == null)
            {
                return;
            }
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.ShowTiles();
            }
        }

        public virtual void HideTile(ObjectId id)
        {
            if (id == null || !m_displayTiles.ContainsKey(id))
            {
                return;
            }
            TileContainer container = m_displayTiles[id];

            if (container != null)
            {
                container.HideTiles();
            }
        }

        public virtual void HideAll()
        {
            foreach (ObjectId id in m_displayTiles.Keys) 
            {
                HideTile(id);
            }
        }

        public virtual void MoveTile(ObjectId id, Vector3 offset)
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

