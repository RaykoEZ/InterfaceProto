using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase 
{

    public class TileSelectionManager : MonoBehaviour
    {
        [SerializeField] GameObject m_highlightObject = default;
        [SerializeField] Tilemap m_map = default;
        [SerializeField] TileManager m_tileHighlightManager = default;
        [SerializeField] BCGameEventTrigger OnPlayerSelect = default;

        public event OnTileSelect OnTileSelected = default;

        static readonly TileId m_highlighterTileId = new TileId(CommandTypes.NONE, "none", "none");

        WorldStateContext m_worldContext;
        CharacterContext m_characterContext;
        Vector3Int m_lastMouseCoordinate = default;

        public void Init(WorldStateContextFactory w, CharacterContextFactory c, TurnOrderManager turnOrder)
        {
            m_worldContext = w.Context;
            w.OnContextChanged += OnWorldUpdate;

            m_characterContext = c.Context;
            c.OnContextChanged += OnCharacterUpdate;

            turnOrder.OnCharacterTurnStart += SelectCharacter;
        }

        void OnWorldUpdate(WorldStateContext world) 
        {
            m_worldContext = world;
        }
        void OnCharacterUpdate(CharacterContext character)
        {
            m_characterContext = character;
        }

        void SelectCharacter(TileId id)
        {
            Vector3 pos = m_characterContext.MemberDataContainer[id].PlayerTransform.position;

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                {"Destination", pos}
            };
            EventInfo info = new EventInfo(id, payload);

            OnTileSelected?.Invoke(info);
            OnPlayerSelect?.TriggerEvent(info);
        }

        void SelectTile(EventInfo info)
        {
            OnTileSelected?.Invoke(info);
            OnPlayerSelect?.TriggerEvent(info);
        }

        void HighlightTileInternal(Vector3 worldPos, Vector3Int gridCoord) 
        {
            if (!m_tileHighlightManager.DoTilesExist(m_highlighterTileId))
            {
                m_tileHighlightManager.SpawnTile(m_highlighterTileId, m_highlightObject, worldPos, transform);
                m_lastMouseCoordinate = gridCoord;
            }
            else
            {
                bool coordMoved = m_lastMouseCoordinate != gridCoord;
                if (coordMoved)
                {
                    Vector3Int diff = gridCoord - m_lastMouseCoordinate;

                    m_tileHighlightManager.MoveTile(m_highlighterTileId, diff);
                    m_lastMouseCoordinate = gridCoord;
                }

            }

            m_tileHighlightManager.ShowTile(m_highlighterTileId);
        }

        public void HandleHighlightTile() 
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridCoord = m_map.WorldToCell(worldPos);
            Vector3 gridWorldPos = m_map.GetCellCenterWorld(gridCoord);

            TileId occupier = m_worldContext.GetOccupyingTileAt(gridCoord);

            Dictionary<string, object> payload = new Dictionary<string, object> 
            {
                {"Destination", gridWorldPos}
            };
            EventInfo info = new EventInfo(occupier, payload);
            SelectTile(info);

            HighlightTileInternal(gridWorldPos, gridCoord);
        }

    }
}

