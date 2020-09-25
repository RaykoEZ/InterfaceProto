using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase 
{
    public abstract class TileBehaviour : MonoBehaviour
    {
        protected OnPlayerCommand<CommandEventInfo> m_onTileCommand = default;
        protected OnCharacterActivate m_onTileSelect = default;
        public TileId m_tileId { get; protected set; }
        protected CharacterData m_charData;
        protected bool m_previewOnly = true;
        public virtual void Init(TileId tileId, CharacterData charData = default) 
        {
            m_tileId = tileId;
            m_charData = charData;
        }

        public virtual void OnStartCommand() { }
        public virtual void OnFinishCommand() { }

        public virtual void Shutdown() 
        {
            m_onTileCommand = null;
            m_onTileSelect = null;
        }

        public void ListenToCommands(OnPlayerCommand<CommandEventInfo> callme)
        {
            m_onTileCommand += callme;
        }

        public void UnlistenToCommands(OnPlayerCommand<CommandEventInfo> callme)
        {
            m_onTileCommand -= callme;
        }

        public void ListenToSelection(OnCharacterActivate callme)
        {
            m_onTileSelect += callme;
        }

        public void UnlistenToSelection(OnCharacterActivate callme)
        {
            m_onTileSelect -= callme;
        }

        public virtual void OnSelect() { }
        public virtual void OnUnselect() { }

        public virtual void OnPlayerPointerHover() { }
        public virtual void OnPlayerPointerExit() { }


    }

    // Basic Event description for a tile interaction. 

}


