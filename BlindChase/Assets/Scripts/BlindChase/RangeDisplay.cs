using System.Collections.Generic;
using UnityEngine;
using BlindChase.Utility;

namespace BlindChase 
{
    // Stores and toggles range display maps
    public class RangeDisplay : MonoBehaviour
    {
        [SerializeField] GameObject m_rangeTile = default;
        [SerializeField] TileSpawner m_spawner = default;

        // Change this later
        Dictionary<NeighbourhoodRangeMap, TileContainer> m_displayTiles = new Dictionary<NeighbourhoodRangeMap, TileContainer>();

        Vector3Int m_previousOrigin = default;

        void Hide(TileContainer tiles) 
        {
            if (tiles != null)
            {
                tiles.HideTiles();
            }
        }

        void Show(NeighbourhoodRangeMap tileOffsets, Vector3Int origin, WorldContext context, TileContainer tiles)
        {
            // If tiles never existed, make new tiles
            if (tiles == null)
            {
                TileContainer newTiles = new TileContainer();

                // This is for showing/creating range tiles.
                foreach (Vector3Int p in tileOffsets.OffsetsFromOrigin)
                {
                    Vector3 offsetPos = context.WorldMap.GetCellCenterLocal(origin + p);

                    GameObject o = m_spawner.SpawnTile(m_rangeTile, offsetPos, transform, false);
                    newTiles.Tiles.Add(o);
                }

                m_displayTiles.Add(tileOffsets, newTiles);
                newTiles.ShowTiles();
                m_previousOrigin = origin;
            }
            else if (origin != m_previousOrigin)
            {
                // Move previous tiles if origin moved
                tiles.MoveTiles(context.WorldMap.GetCellCenterLocal(origin - m_previousOrigin));
                m_previousOrigin = origin;
                tiles.ShowTiles();
            }
            else 
            {
                tiles.ShowTiles();
            }

        }

        public void ToggleRangeDisplay(NeighbourhoodRangeMap tileOffsets, Vector3Int origin, WorldContext context)
        {
            if (m_displayTiles.TryGetValue(tileOffsets, out TileContainer tiles) && tiles.isActive) 
            {
                Hide(tiles);
            }
            else 
            {
                Show(tileOffsets, origin, context, tiles);
            }
        }
    }

}
