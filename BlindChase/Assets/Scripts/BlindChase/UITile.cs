using System;
using UnityEngine;
using BlindChase;
using BlindChase.Events;


public class UITile : TileBehaviour
{
    public override void OnPlayerSelect(WorldContext world = null, PlayerContext player = null) 
    {
        TileEventInfo info = new TileEventInfo(transform.position);

        m_onTileTrigger?.Invoke(info);
    }
}
