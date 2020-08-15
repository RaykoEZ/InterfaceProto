using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{

    public delegate void OnTileTrigger<T>(T info)
        where T : TileEventInfo;

    public delegate void OnTileSelected<T>(T info)
        where T : TileId;

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
        public event OnTileSelected<TileId> OnTileSelect = default;


        public TileContainer(TileItem item) 
        {
            m_tiles?.Add(item);
        }

        protected void OnTileEventTrigger(TileEventInfo info) 
        {
            OnTileTrigger?.Invoke(info);
        }

        protected void OnTileEventSelected(TileId info)
        {
            OnTileSelect?.Invoke(info);
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
                                o.Behaviour?.UnlistenToTileSelection(OnTileEventSelected);

            }
            isActive = false;
        }

        public virtual void ShowTiles() 
        {
            foreach (TileItem o in m_tiles)
            {
                o.TileObject?.SetActive(true);
                o.Behaviour?.ListenToEvents(OnTileEventTrigger);
                o.Behaviour?.ListenToTileSelection(OnTileEventSelected);

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
                o.Behaviour?.UnlistenToTileSelection(OnTileEventSelected);

                Object.Destroy(o.TileObject);
            }
        }
    }
}

