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
        [SerializeField] GameObject m_moveRangeTile = default;
        [SerializeField] GameObject m_skillRangeTile = default;
        [SerializeField] GameObject m_previewRangeTile = default;

        [SerializeField] RangeMapDatabase m_rangeDsplayMasks = default;
        [SerializeField] SkillTargetManager m_skillTargetManager = default;
        [SerializeField] CharacterHUD m_HUD = default;
        [SerializeField] TileManager m_rangeTileManager = default;
        [SerializeField] Tilemap m_map = default;

        [SerializeField] BCGameEventTrigger OnSkillConfirmed = default;
        [SerializeField] BCGameEventTrigger OnPlayerMove = default;
        [SerializeField] BCGameEventTrigger OnPromptCancel = default;

        CharacterContext m_characterContext;
        EventInfo m_latestTileEventInfo;
        TileId m_currentActiveTileId = default;

        CommandTypes m_currentPrompt = CommandTypes.NONE;

        RangeMap m_currentPromptRange;

        public virtual void Init(CharacterContextFactory c, TurnOrderManager turnOrderManager)
        {
            m_skillTargetManager.Init();
            m_HUD.OnRangeCancel += CancelAll;
            turnOrderManager.OnCharacterTurnStart += OnActiveTileIdChange;
            m_skillTargetManager.OnTargetConfirmed += OnSkillTargetsConfirmed;
            m_characterContext = c.Context;
            c.OnContextChanged += OnCharacterContextUpdate;
        }

        public void Shutdown() 
        {
            m_skillTargetManager.Shutdown();
            m_rangeTileManager.Shutdown();
            m_HUD.OnRangeCancel -= CancelAll;
        }
        void OnActiveTileIdChange(TileId id)
        {
            m_currentActiveTileId = id;
        }

        void OnCharacterContextUpdate(CharacterContext context) 
        {
            m_characterContext = context;
        }

        // Handles general user input (range preview and any tile selection prompt made by this handler)
        public void OnPlayerSelect(EventInfo info) 
        {
            if(m_currentPrompt == CommandTypes.NONE && info.SourceId != null) 
            {
                CharacterState state = m_characterContext.MemberDataContainer[info.SourceId].PlayerState;
                Transform parent = m_characterContext.MemberDataContainer[info.SourceId].PlayerTransform;
                ShowRangeMapPreview(info.SourceId, state.Character.ClassType, state.Position, parent, true);
            }
            else if (m_currentPrompt != CommandTypes.NONE)
            {
                OnPromptTileSelected(info);
            }
            else 
            {
                m_rangeTileManager.HideAll();
            }
        }

        // Handles display for movement prompt, when player presses Move button on the HUD
        public void OnMovementPrompt(EventInfo info) 
        {
            bool isMoving = (bool)info.Payload["isMoving"];

            if (m_currentActiveTileId != null && isMoving)
            {
                CharacterState state = m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerState;
                Transform parent = m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerTransform;

                TileId movementRangeTileId = new TileId(CommandTypes.ADVANCE, m_currentActiveTileId.FactionId, m_currentActiveTileId.UnitId);
                m_currentPrompt = CommandTypes.ADVANCE;
                ShowRangeMap(movementRangeTileId, state.Character.ClassType, state.Position, parent, true);
            }
            else 
            {
                CancelAll();            
            }
        }

        public void OnPromptFinish(EventInfo info) 
        {
            m_currentPrompt = CommandTypes.NONE;
        }

        #region Method to handle player inputs from a prompt.

        void OnPromptTileSelected(EventInfo tileEventInfo)
        {
            if (m_currentActiveTileId == null)
            {
                return;
            }

            Vector3 dest = (Vector3)tileEventInfo?.Payload["Destination"];
            if(dest == null) 
            {
                return;
            }

            Vector3Int offset = m_map.WorldToCell(dest) - m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerState.Position;
            bool isSelectionValid = m_currentPromptRange != null && m_currentPromptRange.OffsetsFromOrigin.Contains(offset);

            tileEventInfo.Payload["Origin"] =
                m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerTransform.position;

            switch (m_currentPrompt)
            {
                case CommandTypes.ADVANCE:
                    {
                        CancelAll();
                        bool isStationary = offset == Vector3Int.zero;
                        // If player selects self, cancel the move prompt.
                        if (isStationary || !isSelectionValid) 
                        {
                            CommandEventInfo commandCancelled = new CommandEventInfo(m_currentActiveTileId, m_currentPrompt);
                            OnPromptCancel?.TriggerEvent(commandCancelled);
                            return;
                        }
                        OnPlayerMove?.TriggerEvent(tileEventInfo);
                        m_currentPrompt = CommandTypes.NONE;
                        break;
                    }
                case CommandTypes.SKILL_ACTIVATE:
                    {
                        if (!isSelectionValid) 
                        {
                            Debug.Log("Selected skill target not valid.");
                            return;
                        }

                        m_skillTargetManager.ToggleTarget(dest);
                        m_latestTileEventInfo = tileEventInfo;
                        break;
                    }
                default:
                    break;
            }

        }
        #endregion

        #region Methods to cancel prompt state
        void CancelPrompt(TileId id) 
        {
            m_rangeTileManager.HideTile(id);
        }
        public void CancelAll()
        {
            m_rangeTileManager.HideAll();
        }
        #endregion

        #region Show method used for spawning range tiles
        void Show(TileId id, RangeMap tileOffsets, Vector3 origin, GameObject tileRef, Transform parent, bool forceOverwrite = false)
        {
            bool tilesExist = m_rangeTileManager.DoTilesExist(id);

            if(forceOverwrite && tilesExist) 
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
        #endregion

        #region Show/ToggleRangeMap variants:

        public void ShowRangeMap(
            TileId id,
            CharacterClassType tileClass,
            Vector3 origin,
            Transform parent,
            bool toggle = false)
        {
            RangeMap rangeMap = m_rangeDsplayMasks.GetClassRangeMap(tileClass);
            m_currentPromptRange = rangeMap;
            if (toggle) 
            {
                ToggleDisplay(id,
                    origin,
                    rangeMap,
                    m_moveRangeTile,
                    parent);
            }
            else 
            {
                ShowDisplay(id,
                    origin,
                    rangeMap,
                    m_moveRangeTile,
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
            m_currentPrompt = CommandTypes.SKILL_ACTIVATE;

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
                    m_previewRangeTile,
                    parent);
            }
            else
            {
                ShowDisplay(id,
                    origin,
                    rangeMap,
                    m_previewRangeTile,
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

            m_currentPromptRange = rangeMap;
            m_rangeTileManager.HideAll();

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
                CancelPrompt(id);
                return !tilesActive;
            }
            m_rangeTileManager.HideAll();

            Show(id, rangeMap, origin, tileRef, parent);
            return true;
        }
        #endregion


        void SetCurrentPromptState(CommandTypes newPromptState) 
        { 
            if( m_currentPrompt != CommandTypes.NONE ) 
            {
                CommandEventInfo info = new CommandEventInfo(m_currentActiveTileId, m_currentPrompt);
                OnPromptCancel?.TriggerEvent(info);
            }

            m_currentPrompt = newPromptState;

        }

        // Used to trigger skill activation when player clicks CONFIRM button and targets are sufficient & valid.
        void OnSkillTargetsConfirmed(HashSet<Vector3> targets) 
        {
            m_latestTileEventInfo.Payload["Target"] = targets;
            CancelAll();
            OnSkillConfirmed?.TriggerEvent(m_latestTileEventInfo);
        }
    }

}
