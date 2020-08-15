using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        protected OnTileTrigger<TileEventInfo> m_onTileTrigger = default;
        protected OnTileSelected<TileId> m_onTileSelected = default;

        protected TileId m_tileId;

        public virtual void Init(TileId tileId) 
        {
            m_tileId = tileId;
        }

        public void ListenToEvents(OnTileTrigger<TileEventInfo> callme)
        {
            m_onTileTrigger += callme;
        }

        public void UnlistenToEvents(OnTileTrigger<TileEventInfo> callme)
        {
            m_onTileTrigger -= callme;
        }

        public void ListenToTileSelection(OnTileSelected<TileId> callme) 
        {
            m_onTileSelected += callme;
        }
        public void UnlistenToTileSelection(OnTileSelected<TileId> callme)
        {
            m_onTileSelected -= callme;

        }

        public abstract void OnPlayerSelect();



    }

    // Basic Event description for a tile interaction. 

}


