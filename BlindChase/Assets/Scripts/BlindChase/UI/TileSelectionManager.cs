using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase 
{

    public class TileSelectionManager : MonoBehaviour
    {
        [SerializeField] GameObject m_highlightObject = default;
        [SerializeField] CameraManager m_camera = default;
        [SerializeField] Tilemap m_map = default;
        [SerializeField] TileManager m_tileHighlightManager = default;
        [SerializeField] BCGameEventTrigger OnCharacterSelect = default;
        [SerializeField] BCGameEventTrigger OnCharacterUnselect = default;

        public event OnCharacterTileActivate OnTileSelect = default;
        static readonly TileId m_highlighterTileId = new TileId(CommandTypes.NONE, "none", "none");

        WorldStateContext m_worldRef;
        CharacterTileManager m_characterManagerRef;
        Vector3Int m_lastMouseCoordinate = default;
        TileId m_lastSelectedTileId = default;

        public void Init(WorldStateContextFactory w, CharacterTileManager tileManager, TurnOrderManager turnOrder)
        {
            m_characterManagerRef = tileManager;
            m_worldRef = w.Context;
            w.OnContextChanged += OnWorldUpdate;
            turnOrder.OnCharacterTurnStart += OnPlayerActivate;
        }

        void OnWorldUpdate(WorldStateContext world) 
        {
            m_worldRef = world;
        }

        void OnPlayerActivate(TileId id) 
        {
            SelectPlayerCharacter(id);
        }

        void SelectPlayerCharacter(TileId id) 
        {
            EventInfo info = new EventInfo(id);
            if (id == null || (m_lastSelectedTileId != null && m_lastSelectedTileId == id))
            {
                OnCharacterUnselect?.TriggerEvent(info);
                m_lastSelectedTileId = null;
                return;
            }

            if (id != null)
            {
                OnTileSelect?.Invoke(id);
                OnCharacterSelect?.TriggerEvent(info);
            }
            m_lastSelectedTileId = id;
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
            m_camera.FocusCamera(worldPos);
        }

        public void HandleHighlightTile() 
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridCoord = m_map.WorldToCell(worldPos);

            TileId occupier = m_worldRef.GetOccupyingTileAt(gridCoord);
            SelectPlayerCharacter(occupier);

            Vector3 gridWorldPos = m_map.GetCellCenterWorld(gridCoord);

            HighlightTileInternal(gridWorldPos, gridCoord);
        }

    }
}

