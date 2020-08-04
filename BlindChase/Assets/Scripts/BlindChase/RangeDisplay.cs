using System.Collections.Generic;
using UnityEngine;
using BlindChase.Utility;
using BlindChase.Events;

namespace BlindChase 
{

    // Stores and toggles range display maps
    public class RangeDisplay : MonoBehaviour
    {
        [SerializeField] GameObject m_rangeTile = default;
        [SerializeField] Transform m_rangeTileParent = default;
        [SerializeField] RangeDisplayMasks m_rangeDsplayMasks = default;
        TileManager m_tileManager = new TileManager();

        public event OnTileTrigger<TileEventInfo> OnRangeTileEvent = default;

        void Start()
        {
            Init();
        }


        public virtual void Init() 
        {
            m_tileManager.OnTileEvent += OnRangeTileTrigger;
        }

        void Hide(string id) 
        {
            m_tileManager.HideTile(id);
        }

        void Show(string id, NeighbourhoodRangeMap tileOffsets, Vector3Int origin, WorldContext world, bool tilesExist)
        {
            // If tiles never existed, make new tiles
            if (!tilesExist)
            {
                // This is for showing/creating range tiles.
                foreach (Vector3Int p in tileOffsets.OffsetsFromOrigin)
                {
                    Vector3 offsetPos = world.WorldMap.GetCellCenterLocal(origin + p);
                    m_tileManager.SpawnTile(id, m_rangeTile, offsetPos, m_rangeTileParent, true);
                }


            }

            m_tileManager.ShowTile(id);          

        }

        public void ToggleRangeDisplay(string id, int range, Vector3Int origin, WorldContext context)
        {
            if (string.IsNullOrWhiteSpace(id)) 
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
                NeighbourhoodRangeMap rangeMap = m_rangeDsplayMasks.GetSquareRadiusMap(range);
                Show(id, rangeMap, origin, context, tilesExist);
            }
        }

        void OnRangeTileTrigger(TileEventInfo tileEventInfo) 
        {
            OnRangeTileEvent?.Invoke(tileEventInfo);
        }

    }

}
