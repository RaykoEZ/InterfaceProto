using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public struct TileItem
    {
        public GameObject TileObject;
        public TileBehaviour Behaviour;
    }

    // Stores tile gameobjects generated in a session.
    public class TileContainer
    {
        List<TileItem> m_tiles = new List<TileItem>();
        public bool isActive { get; protected set; } = false;

        public TileContainer() 
        {
        }

        public virtual void AddTileItem(TileItem item) 
        {
            m_tiles.Add(item);
            isActive = item.TileObject.activeSelf;
        }

        public virtual void HideTiles() 
        {
            foreach (TileItem o in m_tiles) 
            {
                o.TileObject?.SetActive(false);
            }
            isActive = false;
        }

        public virtual void ShowTiles() 
        {
            foreach (TileItem o in m_tiles)
            {
                o.TileObject?.SetActive(true);
            }
            isActive = true;
        }

        public virtual void MoveTiles(Vector3 diff) 
        {
            foreach (TileItem o in m_tiles)
            {
                o.TileObject.transform.position += diff;
            }
        }

        public virtual void DestroyTiles() 
        {
            isActive = false;
            foreach (TileItem o in m_tiles) 
            {
                Object.Destroy(o.TileObject);
            }
        }
    }
}

