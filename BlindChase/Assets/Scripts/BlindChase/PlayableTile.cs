using UnityEngine;
using BlindChase;
using BlindChase.Events;


namespace BlindChase 
{
    public class PlayableTile : TileBehaviour
    {
        [SerializeField] RangeDisplay m_rangeDisplay = default;

        public override void Init()
        {
            base.Init();
            m_rangeDisplay.OnRangeTileEvent += OnPlayerTileAction;
        }

        public override void OnPlayerSelect(WorldContext world = null, PlayerContext player = null)
        {
            m_rangeDisplay.ToggleRangeDisplay(TileDisplayKeywords.PLAYER_RANGE, 1, player.PlayerCoord, world);
        }

        public virtual void OnPlayerTileAction(TileEventInfo eventArg)
        {
            m_onTileTrigger?.Invoke(eventArg);
        }
    }

}
