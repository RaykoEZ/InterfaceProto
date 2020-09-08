using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.Events;


public class RangeTile : TileBehaviour
{
    [SerializeField] TileTrigger tileEventTrigger = default;

    public override void Init(TileId tileId, RangeDisplay rangeDisplay, CharacterData charData = default)
    {
        base.Init(tileId, rangeDisplay, charData);
        tileEventTrigger.OnPonterSelected += OnPlayerPointerSelect;
    }

    public override void OnPlayerPointerSelect() 
    {
        Dictionary<string, object> payload = new Dictionary<string, object>();

        CommandEventInfo info = new CommandEventInfo(
            m_tileId, 
            transform.position,
            m_tileId.CommandType, payload);

        m_onTileCommand?.Invoke(info);
    }
}
