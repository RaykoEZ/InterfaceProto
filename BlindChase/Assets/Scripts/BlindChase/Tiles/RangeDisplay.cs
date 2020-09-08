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
        [SerializeField] GameObject m_skillRangeTile = default;
        [SerializeField] RangeMapDatabase m_rangeDsplayMasks = default;
        [SerializeField] SkillTargetManager m_skillTargetManager = default;
        [SerializeField] CharacterHUD m_HUD = default;

        TileManager m_tileManager = new TileManager();
        CommandEventInfo m_latestTileEventInfo;
        public event OnPlayerCommand<CommandEventInfo> OnRangeTileEvent = default;

        TileId m_currentActiveTileId = default;
        public void Shutdown() 
        {
            m_skillTargetManager.Shutdown();
            m_tileManager.Shutdown();
            m_tileManager.OnTileCommand -= OnRangeTileTrigger;
            m_HUD.OnRangeCancel -= HideAll;

            OnRangeTileEvent = null;
        }

        public virtual void Init(TurnOrderManager turnOrderManager) 
        {
            m_skillTargetManager.Init();
            m_HUD.OnRangeCancel += HideAll;
            turnOrderManager.OnCharacterTurnStart += OnActiveTileIdChange;
            m_currentActiveTileId = turnOrderManager.GetActiveCharacterId();
            m_tileManager.OnTileCommand += OnRangeTileTrigger;
            m_skillTargetManager.OnTargetConfirmed += OnSkillTargetsConfirmed;
        }

        void OnActiveTileIdChange(TileId id) 
        {
            m_currentActiveTileId = id;
        }

        void Hide(TileId id) 
        {
            m_tileManager.HideTile(id);
        }

        public void HideAll()
        {
            m_tileManager.HideAll();
        }

        void Show(TileId id, RangeMap tileOffsets, Vector3 origin, GameObject tileRef, Tilemap map, Transform parent, bool forceNew = false)
        {
            bool tilesExist = m_tileManager.DoTilesExist(id);

            if(forceNew && tilesExist) 
            {
                m_tileManager.DespawnTiles(id);
            }

            // If tiles never existed, make new tiles
            if (!m_tileManager.DoTilesExist(id))
            {
                Vector3Int originCoord = map.LocalToCell(origin);
                // This is for showing/creating range tiles.
                foreach (Vector3Int p in tileOffsets.OffsetsFromOrigin)
                {
                    Vector3 offsetPos = map.GetCellCenterLocal(originCoord + p);
                    m_tileManager.SpawnTile(id, tileRef, offsetPos, parent);
                }
            }

            m_tileManager.ShowTile(id);          

        }

        #region Show/ToggleRangeMap variants:
        
        public void ShowRangeMap(
            TileId id,
            CharacterClassType tileClass,
            Vector3 origin,
            Tilemap tilemap,
            Transform parent,
            bool toggle = false)
        {
            RangeMap rangeMap = m_rangeDsplayMasks.GetClassRangeMap(tileClass);
            if (toggle) 
            {
                ToggleDisplay(id,
                    origin,
                    rangeMap,
                    m_rangeTile,
                    tilemap,
                    parent);
            }
            else 
            {
                ShowDisplay(id,
                    origin,
                    rangeMap,
                    m_rangeTile,
                    tilemap,
                    parent);
            }

        }

        public void ShowSkillTargetOption(
            TileId id, 
            string skillRangeId, 
            Vector3 origin, 
            int targetLimit, 
            Tilemap tilemap, 
            Transform parent)
        {
            RangeMap rangeMap = m_rangeDsplayMasks.GetSkillRangeMap(skillRangeId);
            ShowDisplay(id, origin, rangeMap, m_skillRangeTile, tilemap, parent, forceNew: true);
            m_skillTargetManager.SetTargetInfo(targetLimit);
        }

        public void ShowRangeMapPreview(
            TileId id,
            CharacterClassType tileClass,
            Vector3 origin,
            Tilemap tilemap,
            Transform parent,
            bool toggle = false)
        {
            RangeMap rangeMap = m_rangeDsplayMasks.GetClassRangeMap(tileClass);

            if (toggle)
            {
                ToggleDisplay(id,
                    origin,
                    rangeMap,
                    m_rangeTile,
                    tilemap,
                    parent);
            }
            else
            {
                ShowDisplay(id,
                    origin,
                    rangeMap,
                    m_rangeTile,
                    tilemap,
                    parent);
            }
        }

        #endregion

        #region Utility for Show/ToggleRangeMap
        void ShowDisplay(TileId id,
            Vector3 origin,
            RangeMap rangeMap,
            GameObject tileRef,
            Tilemap tilemap,
            Transform parent,
            bool forceNew = false) 
        {
            if (id == null)
            {
                return;
            }
            HideAll();

            Show(id, rangeMap, origin, tileRef, tilemap, parent, forceNew);
        }

        // true - Display is now active
        // false - Display is now not active
        bool ToggleDisplay(
            TileId id, 
            Vector3 origin, 
            RangeMap rangeMap,
            GameObject tileRef,
            Tilemap tilemap,
            Transform parent) 
        {
            if (id == null)
            {
                return false;
            }
            bool tilesActive = m_tileManager.AreTilesActive(id);
            if (tilesActive)
            {
                Hide(id);
                return !tilesActive;
            }
            HideAll();

            Show(id, rangeMap, origin, tileRef, tilemap, parent);
            return true;
        }
        #endregion

        void OnRangeTileTrigger(CommandEventInfo tileEventInfo) 
        {
            if (!TileId.CompareFactionAndUnitId(tileEventInfo.TileId, m_currentActiveTileId)) 
            {
                return;
            }

            if (tileEventInfo.CommandType == CommandTypes.SKILL_ACTIVATE) 
            {
                m_skillTargetManager.ToggleTarget(tileEventInfo.Location);
                m_latestTileEventInfo = tileEventInfo;
                return;
            }
            
            HideAll();
            OnRangeTileEvent?.Invoke(tileEventInfo);
        }


        void OnSkillTargetsConfirmed(HashSet<Vector3> targets) 
        {
            m_latestTileEventInfo.Payload["Target"] = targets;
            HideAll();
            OnRangeTileEvent?.Invoke(m_latestTileEventInfo);
        }
    }

}
