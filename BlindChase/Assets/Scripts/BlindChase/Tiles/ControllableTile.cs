using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class ControllableTile : TileBehaviour
    {
        [SerializeField] RangeDisplay m_rangeDisplay = default;

        public override void Init(TileId tileId)
        {
            base.Init(tileId);
            m_rangeDisplay.OnRangeTileEvent += OnRangeTileTrigger;
        }


        public override void OnPlayerSelect()
        {
            TileId tileId = new TileId(
                TileDisplayKeywords.TILE_HIGHTLIGHT,
                m_tileId.FactionId, 
                m_tileId.UnitId);

            m_rangeDisplay.ToggleRangeDisplay(tileId, 1, transform.position);
            m_onTileSelected?.Invoke(m_tileId);
        }

        public virtual void OnRangeTileTrigger(TileEventInfo eventArg)
        {
            Dictionary<string, object> payload = eventArg.Payload;
            if (payload == null) 
            {
                payload = new Dictionary<string, object>();
            }

            payload.Add("origin", transform.position);

            TileEventInfo overrideArg = new TileEventInfo(m_tileId, eventArg.Location, eventArg.CommandType, payload);
            m_onTileTrigger?.Invoke(overrideArg);
        }
    }

}
