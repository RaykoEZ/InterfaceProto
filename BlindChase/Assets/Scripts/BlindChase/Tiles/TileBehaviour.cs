using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        public TileId m_tileId { get; protected set; }
        protected CharacterData m_charData;
        public virtual void Init(TileId tileId, CharacterData charData = default) 
        {
            m_tileId = tileId;
            m_charData = charData;
        }

        public virtual void OnStartCommand() { }
        public virtual void OnFinishCommand() { }

        public virtual void Shutdown() 
        {
        }

        public virtual void OnSelect() { }
        public virtual void OnUnselect() { }

        public virtual void OnPlayerPointerHover() { }
        public virtual void OnPlayerPointerExit() { }
    }

    // Basic Event description for a tile interaction. 

}


