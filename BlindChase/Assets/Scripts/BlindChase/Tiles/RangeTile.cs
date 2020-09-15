using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.Events;


public class RangeTile : TileBehaviour
{
    [SerializeField] TileTrigger tileEventTrigger = default;

    public override void Init(TileId tileId, CharacterData charData = default)
    {
        base.Init(tileId, charData);
        tileEventTrigger.OnPonterSelected += OnSelect;
    }

    public override void OnSelect() 
    {
        Dictionary<string, object> payload = new Dictionary<string, object>();
        payload["Destination"] = transform.position;

        CommandEventInfo info = new CommandEventInfo(
            m_tileId, 
            m_tileId.CommandType, payload);

        m_onTileCommand?.Invoke(info);
    }
}
