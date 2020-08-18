using System;
using UnityEngine;
using BlindChase;
using BlindChase.Events;


public class UITile : TileBehaviour
{
    public override void OnPlayerSelect() 
    {
        TileEventInfo info = new TileEventInfo(
            m_tileId, 
            transform.position,
            CommandTypes.MOVE);

        m_onTileTrigger?.Invoke(info);
    }
}
