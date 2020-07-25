using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{

    public delegate void OnTileTrigger<T>(T info)
        where T : TileEventInfo;


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

        public event OnTileTrigger<TileEventInfo> OnTileTrigger = default;


        public TileContainer(TileItem item) 
        {
            m_tiles?.Add(item);
        }

        protected void OnTileEventTrigger(TileEventInfo info) 
        {
            OnTileTrigger?.Invoke(info);
        }

        public virtual void AddTileItem(TileItem item) 
        {
            m_tiles.Add(item);
        }

        public virtual void HideTiles() 
        {
            foreach (TileItem o in m_tiles) 
            {
                o.TileObject?.SetActive(false);
                o.Behaviour?.UnlistenToEvents(OnTileEventTrigger);

            }
            isActive = false;
        }

        public virtual void ShowTiles() 
        {
            foreach (TileItem o in m_tiles)
            {
                o.TileObject?.SetActive(true);
                o.Behaviour?.ListenToEvents(OnTileEventTrigger);
            }
            isActive = true;
        }

        public virtual void MoveTiles(Vector3 diff) 
        {
            foreach (TileItem o in m_tiles)
            {
                o.TileObject.transform.position += diff;
            }
            isActive = true;
        }

        public virtual void DestroyTiles() 
        {
            isActive = false;
            foreach (TileItem o in m_tiles) 
            {
                o.Behaviour?.UnlistenToEvents(OnTileEventTrigger);
                Object.Destroy(o.TileObject);
            }
        }
    }
}

