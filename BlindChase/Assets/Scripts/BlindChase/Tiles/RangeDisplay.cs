using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Utility;
using BlindChase.Events;

namespace BlindChase 
{

    // Stores and toggles range display maps
    public class RangeDisplay : MonoBehaviour
    {
        [SerializeField] GameObject m_rangeTile = default;
        [SerializeField] Transform m_rangeTileParent = default;
        [SerializeField] Tilemap m_localTilemap = default;
        [SerializeField] RangeDisplayMasks m_rangeDsplayMasks = default;
        TileManager m_tileManager = new TileManager();

        public event OnTileCommand<TileEventInfo> OnRangeTileEvent = default;

        void Start()
        {
            Init();
        }

        void OnDestroy()
        {
            m_tileManager.Shutdown();
            m_tileManager.OnTileEvent -= OnRangeTileTrigger;
            OnRangeTileEvent = null;
        }


        public virtual void Init() 
        {
            m_tileManager.OnTileEvent += OnRangeTileTrigger;
        }

        void Hide(TileId id) 
        {
            m_tileManager.HideTile(id);
        }

        void Show(TileId id, RangeMap tileOffsets, Vector3 origin, bool tilesExist)
        {
            // If tiles never existed, make new tiles
            if (!tilesExist)
            {
                Vector3Int originCoord = m_localTilemap.LocalToCell(origin);
                // This is for showing/creating range tiles.
                foreach (Vector3Int p in tileOffsets.OffsetsFromOrigin)
                {
                    Vector3 offsetPos = m_localTilemap.GetCellCenterLocal(originCoord + p);
                    m_tileManager.SpawnTile(id, m_rangeTile, offsetPos, m_rangeTileParent, true);
                }
            }

            m_tileManager.ShowTile(id);          

        }

        public void ToggleRangeDisplay(TileId id, int range, Vector3 origin)
        {
            if (id == null) 
            {
                return;
            }

            bool tilesExist = m_tileManager.DoTilesExist(id);
            bool isActive = m_tileManager.AreTilesActive(id);
            if (tilesExist && isActive) 
            {
                Hide(id);
            }
            else 
            {
                RangeMap rangeMap = m_rangeDsplayMasks.GetSquareRadiusMap(range);
                Show(id, rangeMap, origin, tilesExist);
            }
        }

        void OnRangeTileTrigger(TileEventInfo tileEventInfo) 
        {
            ToggleRangeDisplay(tileEventInfo.TileId, 1, transform.position);

            OnRangeTileEvent?.Invoke(tileEventInfo);
        }

    }

}
