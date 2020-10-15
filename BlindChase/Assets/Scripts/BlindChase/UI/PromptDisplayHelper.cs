using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Utility;
using BlindChase.GameManagement;

namespace BlindChase.UI
{
    public class PromptDisplayHelper : MonoBehaviour
    {
        [SerializeField] TileManager m_rangeTileManager = default;
        [SerializeField] Tilemap m_map = default;

        public void Init()
        {
            m_rangeTileManager.Init();
        }

        public void Shutdown() 
        {
            m_rangeTileManager.Shutdown();
        }


        #region Methods to cancel prompt display
        void HidePrompt(ObjectId id)
        {
            m_rangeTileManager.HideTile(id);
        }
        public void HideAll()
        {
            m_rangeTileManager.HideAll();
        }
        #endregion

        #region Show method used for spawning range tiles
        void Show(ObjectId id, RangeMap tileOffsets, Vector3 origin, GameObject tileRef, Transform parent, bool forceOverwrite = false)
        {
            bool tilesExist = m_rangeTileManager.DoTilesExist(id);

            if (forceOverwrite && tilesExist)
            {
                m_rangeTileManager.DespawnTiles(id);
            }

            // If tiles never existed, make new tiles
            if (!m_rangeTileManager.DoTilesExist(id))
            {
                Vector3Int originCoord = m_map.LocalToCell(origin);
                // This is for showing/creating range tiles.
                foreach (Vector3Int p in tileOffsets?.OffsetsFromOrigin)
                {
                    Vector3 offsetPos = m_map.GetCellCenterLocal(originCoord + p);
                    m_rangeTileManager.SpawnTile(id, tileRef, offsetPos, parent);
                }
            }

            m_rangeTileManager.ShowTile(id);

        }
        #endregion

        #region Show/ToggleRangeMap variants:

        public void ShowRangeMap(
            ObjectId id,
            GameObject tileToSpawn,
            RangeMap range,
            Vector3 origin,
            Transform parent,
            bool toggle = false)
        {
            if (toggle)
            {
                ToggleDisplay(id,
                    origin,
                    range,
                    tileToSpawn,
                    parent);
            }
            else
            {
                ShowDisplay(id,
                    origin,
                    range,
                    tileToSpawn,
                    parent);
            }
        }

        #endregion

        #region Utility for Show/ToggleRangeMap
        void ShowDisplay(
            ObjectId id,
            Vector3 origin,
            RangeMap rangeMap,
            GameObject tileRef,
            Transform parent,
            bool forceNew = false)
        {
            if (id == null)
            {
                return;
            }

            m_rangeTileManager.HideAll();

            Show(id, rangeMap, origin, tileRef, parent, forceNew);
        }

        // true - Display is now active
        // false - Display is now not active
        bool ToggleDisplay(
            ObjectId id,
            Vector3 origin,
            RangeMap rangeMap,
            GameObject tileRef,
            Transform parent)
        {
            if (id == null)
            {
                return false;
            }
            bool tilesActive = m_rangeTileManager.AreTilesActive(id);
            if (tilesActive)
            {
                HidePrompt(id);
                return !tilesActive;
            }
            m_rangeTileManager.HideAll();

            Show(id, rangeMap, origin, tileRef, parent);
            return true;
        }
        #endregion
    }

}


