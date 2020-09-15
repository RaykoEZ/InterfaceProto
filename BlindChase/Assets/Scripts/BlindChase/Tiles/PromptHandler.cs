using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Utility;
using BlindChase.Events;

namespace BlindChase 
{

    // Stores and toggles range display maps
    public class PromptHandler : MonoBehaviour
    {
        [SerializeField] GameObject m_rangeTile = default;
        [SerializeField] GameObject m_skillRangeTile = default;
        [SerializeField] RangeMapDatabase m_rangeDsplayMasks = default;
        [SerializeField] SkillTargetManager m_skillTargetManager = default;
        [SerializeField] CharacterHUD m_HUD = default;
        [SerializeField] TileManager m_rangeTileManager = default;
        [SerializeField] Tilemap m_map = default;

        [SerializeField] BCGameEventTrigger OnSkillConfirmed = default;
        [SerializeField] BCGameEventTrigger OnPlayerMove = default;

        CharacterContext m_characterContext;
        EventInfo m_latestTileEventInfo;
        TileId m_currentActiveTileId = default;
        public virtual void Init(CharacterContextFactory c, TurnOrderManager turnOrderManager)
        {
            m_skillTargetManager.Init();
            m_HUD.OnRangeCancel += HideAll;
            turnOrderManager.OnCharacterTurnStart += OnActiveTileIdChange;
            m_currentActiveTileId = turnOrderManager.GetActiveCharacterId();

            m_rangeTileManager.OnTileCommand += OnRangeTileTrigger;
            m_skillTargetManager.OnTargetConfirmed += OnSkillTargetsConfirmed;

            c.OnContextChanged += OnCharacterContextUpdate;
            m_characterContext = c.Context;
        }

        public void Shutdown() 
        {
            m_skillTargetManager.Shutdown();
            m_rangeTileManager.Shutdown();
            m_rangeTileManager.OnTileCommand -= OnRangeTileTrigger;
            m_HUD.OnRangeCancel -= HideAll;
        }

        void OnCharacterContextUpdate(CharacterContext context) 
        {
            m_characterContext = context;
        }

        public void OnPlayerSelect(EventInfo info) 
        {
            CharacterState state = m_characterContext.MemberDataContainer[info.SourceId].PlayerState;
            Transform parent = m_characterContext.MemberDataContainer[info.SourceId].PlayerTransform;


            if(info.SourceId == m_currentActiveTileId) 
            {
                TileId movementRangeTileId = new TileId(CommandTypes.MOVE, info.SourceId.FactionId, info.SourceId.UnitId);
                ShowRangeMap(movementRangeTileId, state.Character.ClassType, state.Position, parent, true);
            }
            else 
            {
                ShowRangeMapPreview(info.SourceId, state.Character.ClassType, state.Position, parent, true);
            }
        }

        public void OnPlayerUnSelect(EventInfo info)
        {
            m_rangeTileManager.HideAll();
        }

        void OnActiveTileIdChange(TileId id) 
        {
            m_currentActiveTileId = id;
        }

        void Hide(TileId id) 
        {
            m_rangeTileManager.HideTile(id);
        }

        public void HideAll()
        {
            m_rangeTileManager.HideAll();
        }

        void Show(TileId id, RangeMap tileOffsets, Vector3 origin, GameObject tileRef, Transform parent, bool forceNew = false)
        {
            bool tilesExist = m_rangeTileManager.DoTilesExist(id);

            if(forceNew && tilesExist) 
            {
                m_rangeTileManager.DespawnTiles(id);
            }

            // If tiles never existed, make new tiles
            if (!m_rangeTileManager.DoTilesExist(id))
            {
                Vector3Int originCoord = m_map.LocalToCell(origin);
                // This is for showing/creating range tiles.
                foreach (Vector3Int p in tileOffsets.OffsetsFromOrigin)
                {
                    Vector3 offsetPos = m_map.GetCellCenterLocal(originCoord + p);
                    m_rangeTileManager.SpawnTile(id, tileRef, offsetPos, parent);
                }
            }

            m_rangeTileManager.ShowTile(id);          

        }

        #region Show/ToggleRangeMap variants:
        
        public void ShowRangeMap(
            TileId id,
            CharacterClassType tileClass,
            Vector3 origin,
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
                    parent);
            }
            else 
            {
                ShowDisplay(id,
                    origin,
                    rangeMap,
                    m_rangeTile,
                    parent);
            }

        }

        public void ShowSkillTargetOption(
            TileId id, 
            string skillRangeId, 
            Vector3 origin, 
            int targetLimit, 
            Transform parent)
        {
            RangeMap rangeMap = m_rangeDsplayMasks.GetSkillRangeMap(skillRangeId);
            ShowDisplay(id, origin, rangeMap, m_skillRangeTile, parent, forceNew: true);
            m_skillTargetManager.SetTargetInfo(targetLimit);
        }

        public void ShowRangeMapPreview(
            TileId id,
            CharacterClassType tileClass,
            Vector3 origin,
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
                    parent);
            }
            else
            {
                ShowDisplay(id,
                    origin,
                    rangeMap,
                    m_rangeTile,
                    parent);
            }
        }

        #endregion

        #region Utility for Show/ToggleRangeMap
        void ShowDisplay(TileId id,
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
            HideAll();

            Show(id, rangeMap, origin, tileRef, parent, forceNew);
        }

        // true - Display is now active
        // false - Display is now not active
        bool ToggleDisplay(
            TileId id, 
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
                Hide(id);
                return !tilesActive;
            }
            HideAll();

            Show(id, rangeMap, origin, tileRef, parent);
            return true;
        }
        #endregion

        void OnRangeTileTrigger(CommandEventInfo tileEventInfo) 
        {
            if (!TileId.CompareFactionAndUnitId(tileEventInfo.SourceId, m_currentActiveTileId)) 
            {
                return;
            }


            tileEventInfo.Payload["Origin"] =
                m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerTransform.position;

            switch (tileEventInfo.CommandType)
            {
                case CommandTypes.MOVE:
                    {
                        HideAll();
                        OnPlayerMove?.TriggerEvent(tileEventInfo);
                        break;
                    }
                case CommandTypes.MOVE_PROMPT:
                    break;
                case CommandTypes.SKILL_ACTIVATE:
                    {
                        m_skillTargetManager.ToggleTarget((Vector3)tileEventInfo.Payload["Destination"]);
                        m_latestTileEventInfo = tileEventInfo;
                        break;
                    }
                default:
                    break;
            }
            

        }

        void OnSkillTargetsConfirmed(HashSet<Vector3> targets) 
        {
            m_latestTileEventInfo.Payload["Target"] = targets;
            HideAll();
            OnSkillConfirmed?.TriggerEvent(m_latestTileEventInfo);
        }
    }

}
