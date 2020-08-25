using System;
using UnityEngine;
using BlindChase;
using BlindChase.Events;


public class RangeTile : TileBehaviour
{
    public override void OnPlayerSelect() 
    {
        TileEventInfo info = new TileEventInfo(
            m_tileId, 
            transform.position,
            m_tileId.TypeId);

        m_onTileCommand?.Invoke(info);
    }
}
