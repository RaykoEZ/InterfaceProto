using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Ai;
using BlindChase.Events;
using BlindChase.State;
using BlindChase.UI;

namespace BlindChase.GameManagement
{
    // Used by PlayerCommand to execute different actions in a player's turn
    public class BCCharacterController : MonoBehaviour
    {
        WorldContext m_worldRef;
        CharacterContext m_characterRef;
        DelayedEffectHandler m_delayedEffects;
        ActionSequenceHandler m_actionSequencer;

        // The id of the faction member tile object we are controlling.
        CharacterState m_currentTargetState;
        WorldStateContextFactory m_worldContextFactoryRef;
        CharacterContextFactory m_characterContextFactoryRef;
        TurnProgressManager m_gameStateManagerRef;
        SkillManager m_skillManagerRef;
        NpcManager m_npcManagerRef;

        public delegate void OnCommandFinished();
        public event OnCommandFinished OnCommandFinish = default;

        [SerializeField] PromptHandler m_prompter = default;

        [SerializeField] BCGameEventTrigger OnCharacterDefeated = default;
        [SerializeField] BCGameEventTrigger OnLeaderDefeated = default;
        [SerializeField] BCGameEventTrigger OnCharacterAdvance = default;
        [SerializeField] BCGameEventTrigger OnSkillActivated = default;

        public void Init(
            CharacterContextFactory c,
            WorldStateContextFactory w,
            TurnOrderManager turnOrder,
            NpcManager npc,
            TurnProgressManager state,
            CharacterManager tilemanager 
            ) 
        {
            m_npcManagerRef = npc;

            m_actionSequencer = new ActionSequenceHandler();
            m_actionSequencer.Init(tilemanager);
            m_actionSequencer.OnCharacterDefeat += OnCharacterDefeat;

            m_delayedEffects = new DelayedEffectHandler();
            m_delayedEffects.OnDelayedEffectActivate += OnDelayedEffectActivate;

            m_worldRef = w.Context;
            m_characterRef = c.Context;
            c.OnContextChanged += OnCharacterContextUpdate;
            m_characterContextFactoryRef = c;
            w.OnContextChanged += OnWorldUpdate;
            m_worldContextFactoryRef = w;

            m_gameStateManagerRef = state;
            m_gameStateManagerRef.OnTurnEnd += OnTurnEnd;

            turnOrder.OnTurnStart += OnCharacterTurnStart;
        }

        public void Shutdown() 
        {
            m_delayedEffects.OnDelayedEffectActivate -= OnDelayedEffectActivate;
            m_characterContextFactoryRef.OnContextChanged -= OnCharacterContextUpdate;
            m_worldContextFactoryRef.OnContextChanged -= OnWorldUpdate;
            m_actionSequencer.OnCharacterDefeat -= OnCharacterDefeat;
            m_gameStateManagerRef.OnTurnEnd -= OnTurnEnd;
        }

        public void AdvancePlayer(Vector3 destination)
        {
            Vector3Int targetCoord = m_worldRef.WorldMap.WorldToCell(destination);
            GameContextRecord context = new GameContextRecord(m_worldRef, m_characterRef);
            SimulationResult result = SkillManager.BasicMovement(context, targetCoord, m_currentTargetState.ObjectId);
            Debug.Log(result.Message);
            HandleResult(m_currentTargetState.ObjectId, result, OnAdvancing, m_actionSequencer.OnCharacterAdvance, endTurn: true);
        }

        public void ActivateSkill(int skillId, int skillLevel, HashSet<Vector3> targetPos, Vector3 userPos)
        {
            List<Vector3Int> targetCoords = new List<Vector3Int>();
            foreach (Vector3 pos in targetPos)
            {
                Vector3Int coord = m_worldRef.WorldMap.WorldToCell(pos);
                targetCoords.Add(coord);
            }

            Vector3Int userCoord = m_worldRef.WorldMap.WorldToCell(userPos);
            ObjectId userId = m_worldRef.GetOccupyingTileAt(userCoord);
            GameContextRecord context = new GameContextRecord(m_worldRef, m_characterRef);
            SimulationResult result = SkillManager.ActivateSkill(skillId, skillLevel, context, targetCoords, userId);
            Debug.Log(result.Message);

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                {"SkillId", skillId}
            };

            HandleResult(userId, result, OnSkilllActivate, m_actionSequencer.OnCharacterSkill, endTurn: true, payload);
        }

        // If player can use this skill, prompt target selection/confirmation
        // If skill not usable, prompt a GUI message
        public void PromptSkillTargetSelection(int skillId, int skillLevel)
        {
            SkillParameters skillData = SkillManager.GetSkillParameters(skillId, skillLevel);
            int cost = skillData.SkillCost;
            //Player cannot use this skill, return.
            if (!SkillManager.CheckSkillPreconditions(m_currentTargetState, skillId, cost, out string message))
            {
                Debug.Log(message);
                return;
            }

            // Prompt Skill target selection here
            string rangeId = skillData.EffectRange;
            int targetLimit = skillData.TargetLimit;

            ObjectId tileId = new ObjectId(
                m_currentTargetState.ObjectId.FactionId,
                m_currentTargetState.ObjectId.UnitId);
            m_prompter.OnSkillPrompt(tileId, skillId, rangeId, targetLimit);
        }


        // Triggers events after player actions' results are calculated.
        void HandleResult(
            ObjectId id, 
            SimulationResult result, 
            Action<EventInfo> onSuccessGameEventTrigger, 
            Action<ActionParticipants, Action, Dictionary<string,object>> actionSequenceResolver,
            bool endTurn = false, 
            Dictionary<string, object> payload = null) 
        {
                // This is called when the player phase animations are finished.
            Action onTransitionFinish = () => 
            { 
                UpdateGame(result.ResulContext);
                OnCommandFinish?.Invoke();
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

        #region Event handles

        void UpdateGame(GameContextRecord context) 
        {
            m_characterContextFactoryRef.Update(context.CharacterRecord);
            m_worldContextFactoryRef.Update(context.WorldRecord);
        }

        void OnCharacterDefeat(EventInfo info)
        {
            OnCharacterDefeated?.TriggerEvent(info);
            CharacterClassType defeatedCharacterClass = m_characterRef
                .MemberDataContainer[info.SourceId].PlayerState.Character.ClassType;

            if (defeatedCharacterClass == CharacterClassType.Master)
            {
                OnLeaderIsDefeated(info);
            }
        }

        void OnLeaderIsDefeated(EventInfo info) 
        {
            OnLeaderDefeated?.TriggerEvent(info);
        }

        void OnAdvancing(EventInfo info)
        {
            OnCharacterAdvance?.TriggerEvent(info);
        }

        void OnSkilllActivate(EventInfo info)
        {
            OnSkillActivated?.TriggerEvent(info);
        }

        // When the player selects a member tile on the map, we start controlling the character
        void OnCharacterTurnStart(ObjectId tileId)
        {
            m_currentTargetState = m_characterRef.MemberDataContainer[tileId].PlayerState;
            GameContextRecord context = new GameContextRecord(m_worldRef, m_characterRef);
            SimulationResult result = SkillManager.AutoRecovery(context, m_currentTargetState.ObjectId);

            m_characterContextFactoryRef.Update(result.ResulContext.CharacterRecord);

            m_delayedEffects.OnTurnStartEffects();

            if (!string.IsNullOrEmpty(tileId.NPCId)) 
            {
                m_npcManagerRef.OnNPCActive(tileId);
            }
        }

        void OnTurnEnd()
        {
            m_delayedEffects.OnTurnEndEffects();
        }

        void OnWorldUpdate(WorldContext world)
        {
            m_worldRef = world;
        }

        void OnDelayedEffectActivate(SimulationResult effectResult) 
        {
            Debug.LogWarning(" Delayed Effects Not implemented YET!!!!!!!!!!");
        }

        void OnCharacterContextUpdate(CharacterContext newContext)
        {
            m_characterRef = newContext;
        }
        #endregion

    }

}


