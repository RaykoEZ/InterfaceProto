using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Utility;
using BlindChase.Events;
using BlindChase.GameManagement;

namespace BlindChase.UI 
{
    // Stores and toggles range display maps
    public class PromptHandler : MonoBehaviour
    {
        [SerializeField] GameObject m_moveRangeTile = default;
        [SerializeField] GameObject m_skillRangeTile = default;
        [SerializeField] GameObject m_previewRangeTile = default;

        [SerializeField] CharacterHUD m_HUD = default;
        [SerializeField] Tilemap m_map = default;
        [SerializeField] BCGameEventTrigger OnSkillConfirmed = default;
        [SerializeField] BCGameEventTrigger OnPlayerMove = default;
        [SerializeField] BCGameEventTrigger OnPromptCancel = default;
        [SerializeField] PromptDisplayHelper m_display = default;
        [SerializeField] RangeMapDatabase m_rangeDsplayMasks = default;

        CharacterContext m_characterContext;
        EventInfo m_latestTileEventInfo;
        ObjectId m_currentActiveTileId = default;
        RangeMap m_currentPromptRange;

        int m_targetLimit = 0;
        HashSet<Vector3> m_targets = new HashSet<Vector3>();

        CommandTypes m_currentPrompt = CommandTypes.NONE;

        public virtual void Init(CharacterContextFactory c, TurnOrderManager turnOrderManager)
        {
            m_display.Init();

            turnOrderManager.OnTurnStart += OnActiveTileIdChange;
            m_characterContext = c.Context;
            c.OnContextChanged += OnCharacterContextUpdate;

            m_HUD.OnRangeCancel += CancelAllPrompt;
            m_HUD.OnRangeCancel += TargetsCancelled;
            m_HUD.OnRangeConfirm += TargetsConfirmed;
        }

        public void Shutdown() 
        {
            m_display.Shutdown();
            m_HUD.OnRangeCancel -= CancelAllPrompt;
            m_HUD.OnRangeCancel -= TargetsCancelled;
            m_HUD.OnRangeConfirm -= TargetsConfirmed;
        }

        void OnActiveTileIdChange(ObjectId id)
        {
            m_currentActiveTileId = id;
        }

        void OnCharacterContextUpdate(CharacterContext context)
        {
            m_characterContext = context;
        }

        public void OnTargetSelected(Vector3 target)
        {
            // Check if the selected destination is valid for the skill.
            //IMPL

            if (m_targets.Contains(target))
            {
                m_targets.Remove(target);
            }
            else if (m_targets.Count < m_targetLimit)
            {
                m_targets.Add(target);
            }

            m_HUD.SetSkillConfirmation(IsTargetLimitSatisfied());
        }

        public void TargetsConfirmed()
        {
            if (m_targets.Count > 0)
            {
                OnSkillTargetsConfirmed(m_targets);
                m_targets.Clear();
                m_targetLimit = 0;
            }
            else
            {
                Debug.LogError("Please select your targets.");
            }
        }
        // Used to trigger skill activation when player clicks CONFIRM button and targets are sufficient & valid.
        public void OnSkillTargetsConfirmed(HashSet<Vector3> targets)
        {
            m_latestTileEventInfo.Payload["Target"] = targets;
            CancelAllPrompt();
            OnSkillConfirmed?.TriggerEvent(m_latestTileEventInfo);
        }
        void TargetsCancelled()
        {
            m_targets.Clear();
        }

        bool IsTargetLimitSatisfied()
        {
            return m_targets.Count <= m_targetLimit && m_targets.Count > 0;
        }

        void ShowPrompt(ObjectId id, GameObject tileToSpawn, RangeMap range, Vector3 worldPos, Transform parent, bool toggle = false)
        {
            m_currentPromptRange = range;
            m_display.ShowRangeMap(id, tileToSpawn, m_currentPromptRange, worldPos, parent, toggle);

        }

        void SetCurrentPromptState(CommandTypes newPromptState)
        {
            if (m_currentPrompt != CommandTypes.NONE && m_currentPrompt != newPromptState)
            {
                CommandRequest command = new CommandRequest(m_currentPrompt);
                CommandRequestInfo info = new CommandRequestInfo(m_currentActiveTileId, command);
                OnPromptCancel?.TriggerEvent(info);
            }

            m_currentPrompt = newPromptState;
        }

        void CancelAllPrompt()
        {
            SetCurrentPromptState(CommandTypes.NONE);
            m_display.HideAll();
        }

        public void OnSkillPrompt(ObjectId id, string skillRangeId, int targetLimit)
        {
            m_targetLimit = targetLimit;
            SetCurrentPromptState(CommandTypes.SKILL_ACTIVATE);
            Transform parent = m_characterContext.MemberDataContainer[id].PlayerTransform;
            Vector3 origin = parent.position;
            RangeMap rangeMap = m_rangeDsplayMasks.GetSkillRangeMap(skillRangeId);
            ObjectId skillRangeTileId = new ObjectId(id.FactionId, id.UnitId, CommandTypes.SKILL_ACTIVATE);

            ShowPrompt(skillRangeTileId, m_skillRangeTile, rangeMap, origin, parent);
        }


        // Handles general user input (range preview and any tile selection prompt made by this handler)
        public void OnPlayerSelect(EventInfo info) 
        {
            if(m_currentPrompt == CommandTypes.NONE && info.SourceId != null) 
            {
                CharacterState state = m_characterContext.MemberDataContainer[info.SourceId].PlayerState;
                Transform parent = m_characterContext.MemberDataContainer[info.SourceId].PlayerTransform;

                ObjectId previewTileId = new ObjectId(info.SourceId.FactionId, info.SourceId.UnitId, m_currentPrompt);
                RangeMap rangeMap = m_rangeDsplayMasks.GetClassRangeMap(state.Character.ClassType);

                ShowPrompt(previewTileId, m_previewRangeTile, rangeMap, parent.position, parent, true);
            }
            else if (m_currentPrompt != CommandTypes.NONE)
            {
                OnPromptTileSelected(info);
            }
            else 
            {
                CancelAllPrompt();
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
                SetCurrentPromptState(CommandTypes.ADVANCE);

                ObjectId movementRangeTileId = new ObjectId(
                    m_currentActiveTileId.FactionId, 
                    m_currentActiveTileId.UnitId,
                    m_currentPrompt);

                RangeMap rangeMap = m_rangeDsplayMasks.GetClassRangeMap(state.Character.ClassType);
                ShowPrompt(movementRangeTileId, m_moveRangeTile, rangeMap, parent.position, parent, true);
            }
            else 
            {
                CancelAllPrompt();            
            }
        }

        public void OnPromptFinish(EventInfo info) 
        {
            CancelAllPrompt();
        }

        #region Method to handle player inputs from a prompt.

        void OnPromptTileSelected(EventInfo tileEventInfo)
        {
            if (m_currentActiveTileId == null)
            {
                return;
            }

            Vector3 dest = (Vector3)tileEventInfo?.Payload["Destination"];
            if (dest == null) 
            {
                return;
            }

            Vector3Int origin = m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerState.Position;
            Vector3Int destCoord = m_map.WorldToCell(dest);
            bool isSelectionValid = m_currentPromptRange.IsInRange(origin, destCoord);            
            if (!isSelectionValid) 
            {
                Debug.Log("Selected target not valid.");
                return;
            }

            Vector3 worldPos = m_characterContext.MemberDataContainer[m_currentActiveTileId].PlayerTransform.position;
            tileEventInfo.Payload["Origin"] = worldPos;

            switch (m_currentPrompt)
            {
                case CommandTypes.ADVANCE:
                    {
                        CancelAllPrompt();
                        Vector3Int offset = m_map.WorldToCell(dest) - origin;
                        bool isStationary = offset == Vector3Int.zero;
                        // If player selects self, cancel the move prompt.
                        if (isStationary) 
                        {
                            CommandRequest command = new CommandRequest(m_currentPrompt);
                            CommandRequestInfo commandCancelled = new CommandRequestInfo(m_currentActiveTileId, command);
                            OnPromptCancel?.TriggerEvent(commandCancelled);
                        }
                        else 
                        {
                            OnPlayerMove?.TriggerEvent(tileEventInfo);
                            SetCurrentPromptState(CommandTypes.NONE);
                        }
                        break;
                    }
                case CommandTypes.SKILL_ACTIVATE:
                    {
                        //IMPL
                        ObjectId selectedId = tileEventInfo.SourceId;
                        AllowedTarget selectionType = TargetingValidation.GetSelectionType(selectedId, m_currentActiveTileId);
                        OnTargetSelected(dest);
                        m_latestTileEventInfo = tileEventInfo;
                        break;
                    }
                default:
                    break;
            }

        }
        #endregion


    }

}
