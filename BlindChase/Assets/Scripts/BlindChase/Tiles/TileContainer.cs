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

    public struct CharacterTileItem
    {
        public GameObject TileObject;
        public CharacterBehaviour Behaviour;
    }

    // Stores tile gameobjects generated in a session.
    public class TileContainer
    {
        List<TileItem> m_tiles = new List<TileItem>();
        public bool isActive { get; protected set; } = false;

        public event OnPlayerCommand<CommandEventInfo> OnTileTrigger = default;
        public event OnCharacterTileActivate OnPlayerSelect = default;

        public TileContainer() 
        {
        }

        protected virtual void OnTileCommandTrigger(CommandEventInfo info) 
        {
            OnTileTrigger?.Invoke(info);
        }

        protected void OnTileSelected(TileId info)
        {
            OnPlayerSelect?.Invoke(info);
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
                o.Behaviour?.UnlistenToCommands(OnTileCommandTrigger);
                o.Behaviour?.UnlistenToSelection(OnTileSelected);
            }
            isActive = false;
        }

        public virtual void ShowTiles() 
        {
            foreach (TileItem o in m_tiles)
            {
                o.TileObject?.SetActive(true);
                o.Behaviour?.ListenToCommands(OnTileCommandTrigger);
                o.Behaviour?.ListenToSelection(OnTileSelected);

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
                o.Behaviour?.UnlistenToCommands(OnTileCommandTrigger);
                o.Behaviour?.UnlistenToSelection(OnTileSelected);
                Object.Destroy(o.TileObject);
            }
        }
    }
}

