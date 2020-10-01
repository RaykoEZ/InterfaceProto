using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.State;

namespace BlindChase
{
    public struct GameContextCollection
    {
        public WorldStateContext World;
        public CharacterContext Characters;
    }

    // Used by PlayerCommand to execute different actions in a player's turn
    public class PlayerController : MonoBehaviour
    {
        GameContextCollection m_gameContext = new GameContextCollection();
        DelayedEffectHandler m_delayedEffects;
        ActionSequenceHandler m_actionSequencer;

        // The id of the faction member tile object we are controlling.
        CharacterState m_currentTargetState;
        WorldStateContextFactory m_worldContextFactoryRef;
        CharacterContextFactory m_characterContextFactoryRef;
        GameStateManager m_gameStateManagerRef;
        SkillManager m_skillManagerRef;



        [SerializeField] PromptHandler m_rangeDisplay = default;

        [SerializeField] BCGameEventTrigger OnCharacterDefeated = default;
        [SerializeField] BCGameEventTrigger OnLeaderDefeated = default;

        [SerializeField] BCGameEventTrigger OnCharacterAdvance = default;
        [SerializeField] BCGameEventTrigger OnSkillActivated = default;
        [SerializeField] BCGameEventTrigger OnCommandFail = default;

        public void Init(
            CharacterContextFactory c,
            WorldStateContextFactory w,
            TurnOrderManager turnOrder,
            SkillManager skillManager,
            GameStateManager state,
            CharacterManager tilemanager 
            ) 
        {
            m_actionSequencer = new ActionSequenceHandler();
            m_actionSequencer.Init(tilemanager);

            m_delayedEffects = new DelayedEffectHandler();
            m_delayedEffects.OnDelayedEffectActivate += OnDelayedEffectActivate;

            c.OnContextChanged += OnCharacterContextUpdate;
            m_characterContextFactoryRef = c;
            m_gameContext.Characters = m_characterContextFactoryRef.Context;

            w.OnContextChanged += OnWorldUpdate;
            m_worldContextFactoryRef = w;
            m_gameContext.World = w.Context;

            m_skillManagerRef = skillManager;
            m_skillManagerRef.OnCharacterDefeat += OnCharacterDefeat;

            m_gameStateManagerRef = state;
            m_gameStateManagerRef.OnTurnEnd += OnTurnEnd;

            turnOrder.OnCharacterTurnStart += OnCharacterTurnStart;
        }

        public void Shutdown() 
        {
            m_delayedEffects.OnDelayedEffectActivate -= OnDelayedEffectActivate;
            m_characterContextFactoryRef.OnContextChanged -= OnCharacterContextUpdate;
            m_worldContextFactoryRef.OnContextChanged -= OnWorldUpdate;
            m_skillManagerRef.OnCharacterDefeat -= OnCharacterDefeat;
            m_gameStateManagerRef.OnTurnEnd -= OnTurnEnd;
        }

        public void AdvancePlayer(Vector3 destination)
        {
            Vector3Int targetCoord = m_gameContext.World.WorldMap.WorldToCell(destination);
            EffectResult result = m_skillManagerRef.BasicMovement(m_gameContext, targetCoord, m_currentTargetState.TileId);
            Debug.Log(result.Message);
            HandleResult(m_currentTargetState.TileId, result, OnAdvancing, m_actionSequencer.OnCharacterAdvance, endTurn: true);
        }

        public void ActivateSkill(int skillId, int skillLevel, HashSet<Vector3> targetPos, Vector3 userPos)
        {
            List<Vector3Int> targetCoords = new List<Vector3Int>();
            foreach (Vector3 pos in targetPos)
            {
                Vector3Int coord = m_gameContext.World.WorldMap.WorldToCell(pos);
                targetCoords.Add(coord);
            }

            Vector3Int userCoord = m_gameContext.World.WorldMap.WorldToCell(userPos);
            TileId userId = m_gameContext.World.GetOccupyingTileAt(userCoord);

            EffectResult result = m_skillManagerRef.ActivateSkill(skillId, skillLevel, m_gameContext, targetCoords, userId);
            Debug.Log(result.Message);

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                {"SkillId", skillId}
            };

            HandleResult(userId, result, OnSkilllActivate, m_actionSequencer.OnCharacterSkill, endTurn: false, payload);
        }

        // If player can use this skill, prompt target selection/confirmation
        // If skill not usable, prompt a GUI message
        public void PromptSkillTargetSelection(int skillId, int skillLevel)
        {
            SkillValueCollection skillValues = SkillManager.GetSkillData(skillId).ValueCollection;
            SkillDataItem skillData = skillValues.SkillValues[skillLevel];
            int cost = skillData.SkillCost;
            //Player cannot use this skill, return.
            if (!PreSkillActivationChecks(skillId, cost))
            {
                // IMPLEMENT OnPromptFail HERE
                return;
            }

            // Prompt Skill target selection here
            string rangeId = skillData.EffectRange;
            int targetLimit = SkillManager.GetSkillTargetLimit(skillId, skillLevel);

            Transform parent = m_gameContext.Characters.MemberDataContainer[m_currentTargetState.TileId].PlayerTransform;
            TileId tileId = new TileId(
                CommandTypes.SKILL_ACTIVATE,
                m_currentTargetState.TileId.FactionId,
                m_currentTargetState.TileId.UnitId);
            m_rangeDisplay.ShowSkillTargetOption(tileId, rangeId, parent.position, targetLimit, parent);
        }


        // Triggers events after player actions' results are calculated.
        void HandleResult(
            TileId id, 
            EffectResult result, 
            Action<EventInfo> onSuccessGameEventTrigger, 
            Action<ActionParticipants, Action, Dictionary<string,object>> actionSequenceResolver,
            bool endTurn = false, 
            Dictionary<string, object> payload = null) 
        {
            if (result.IsSuccessful) 
            {
                // This is called when the player phase animations are finished.
                Action onTransitionFinish = () => 
                { 
                    UpdateGame(result.ResulContext);
                    if (endTurn) 
                    {
                        m_gameStateManagerRef.TransitionToNextState();
                    }
                };

                ActionParticipants participants = new ActionParticipants 
                { 
                    InitiateId = id, 
                    Affected = result.AffectedCharacters 
                };

                actionSequenceResolver?.Invoke(participants, onTransitionFinish, payload);

                EventInfo eventInfo = new EventInfo(id, payload);
                onSuccessGameEventTrigger?.Invoke(eventInfo);
            }
            // IMPLEMENT OnPromptFail HERE
            else
            {
                EventInfo info = new EventInfo(id);
                OnCommandFail?.TriggerEvent(info);
            }
        }

        #region Event handles

        void UpdateGame(GameContextCollection context) 
        {
            m_characterContextFactoryRef.Update(context.Characters);
            m_worldContextFactoryRef.Update(context.World);
        }

        public void OnCharacterDefeat(EventInfo info)
        {
            OnCharacterDefeated?.TriggerEvent(info);
            CharacterClassType defeatedCharacterClass = m_gameContext.
                Characters.MemberDataContainer[info.SourceId].PlayerState.Character.ClassType;

            if (defeatedCharacterClass == CharacterClassType.Master)
            {
                OnLeaderIsDefeated(info);
            }
        }

        public void OnLeaderIsDefeated(EventInfo info) 
        {
            OnLeaderDefeated?.TriggerEvent(info);
        }

        public void OnAdvancing(EventInfo info)
        {
            OnCharacterAdvance?.TriggerEvent(info);
        }

        public void OnSkilllActivate(EventInfo info)
        {
            OnSkillActivated?.TriggerEvent(info);
        }

        // When the player selects a member tile on the map, we start controlling the character
        void OnCharacterTurnStart(TileId tileId)
        {
            m_currentTargetState = m_gameContext.Characters.MemberDataContainer[tileId].PlayerState;

            EffectResult result = m_skillManagerRef.AutoRecovery(m_gameContext, m_currentTargetState.TileId);

            m_characterContextFactoryRef.Update(result.ResulContext.Characters);

            m_delayedEffects.OnTurnStartEffects();
        }

        void OnTurnEnd()
        {
            if (m_currentTargetState == null)
            {
                return;
            }

            m_delayedEffects.OnTurnEndEffects();
        }

        void OnWorldUpdate(WorldStateContext world)
        {
            m_gameContext.World = world;
        }

        void OnDelayedEffectActivate(EffectResult effectResult) 
        {
            Debug.LogWarning(" Delayed Effects Not implemented YET!!!!!!!!!!");
        }

        void OnCharacterContextUpdate(CharacterContext newContext)
        {
            m_gameContext.Characters = newContext;
        }
        #endregion

        bool PreSkillActivationChecks(int skillId, int cost) 
        {
            if (!m_currentTargetState.CurrentSkillCooldowns.ContainsKey(skillId))
            {
                Debug.Log("Invalid skill");
                return false;
            }

            // If skill on cooldown or skillId invalid, nope go back
            if (m_currentTargetState.CurrentSkillCooldowns[skillId] > 0)
            {
                //Prompt message : Skill on cooldown
                Debug.Log("Skill still on cooldown.");
                return false;
            }

            if (m_currentTargetState.CurrentSP < cost) 
            {
                Debug.Log("Insufficient SP.");
                return false;
            }

            return true;
        }

    }

}


