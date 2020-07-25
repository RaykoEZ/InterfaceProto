using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        protected OnTileTrigger<TileEventInfo> m_onTileTrigger = default;

        public virtual void Init() 
        {
        }


        public void ListenToEvents(OnTileTrigger<TileEventInfo> callme)
        {
            m_onTileTrigger += callme;
        }

        public void UnlistenToEvents(OnTileTrigger<TileEventInfo> callme)
        {
            m_onTileTrigger -= callme;
        }

        public abstract void OnPlayerSelect(WorldContext world = null, PlayerContext player = null);

    }

    // Basic Event description for a tile interaction. 

}


