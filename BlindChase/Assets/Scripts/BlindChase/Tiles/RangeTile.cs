using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.Events;


public class RangeTile : TileBehaviour
{
    public override void Init(TileId tileId, CharacterData charData = default)
    {
        base.Init(tileId, charData);
        #region Unused code
        /*Collider[] overlaps = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);

        if(overlaps.Length == 0) 
        {
            return;
        }
        foreach (Collider collider in overlaps) 
        {
            TileBehaviour tile = collider.gameObject.GetComponent<TileBehaviour>();

            bool isObstructed = tile != null && (tile.m_tileId.FactionId == m_tileId.FactionId);       
            if (isObstructed)
            {
                m_sprite.color = new Color(1f,1f,0f,0.5f);
                break;
            }
        }*/
        #endregion
    }
}
