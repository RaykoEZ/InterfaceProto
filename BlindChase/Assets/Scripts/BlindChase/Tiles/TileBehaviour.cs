using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        protected OnTileCommand<TileEventInfo> m_onTileCommand = default;
        protected OnTileSelected<TileId> m_onTileSelected = default;

        protected TileId m_tileId;

        public virtual void Init(TileId tileId) 
        {
            m_tileId = tileId;
        }

        public void ListenToCommands(OnTileCommand<TileEventInfo> callme)
        {
            m_onTileCommand += callme;
        }

        public void UnlistenToCommands(OnTileCommand<TileEventInfo> callme)
        {
            m_onTileCommand -= callme;
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


