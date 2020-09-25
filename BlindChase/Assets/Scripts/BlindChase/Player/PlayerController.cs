using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
        CharacterTileManager m_tileManager;
        GameContextCollection m_gameContext = new GameContextCollection();
        // The id of the faction member tile object we are controlling.
        TileId m_targetId;
        CharacterState m_currentTargetState;
        WorldStateContextFactory m_worldContextFactoryRef;
        CharacterContextFactory m_characterContextFactoryRef;
        GameStateManager m_gameStateManagerRef;
        SkillManager m_skillManagerRef;

        [SerializeField] PromptHandler m_rangeDisplay = default;
        [SerializeField] BCGameEventTrigger OnCharacterDefeated = default;
        [SerializeField] BCGameEventTrigger OnLeaderDefeated = default;
        [SerializeField] BCGameEventTrigger OnCharacterAttack = default;
        [SerializeField] BCGameEventTrigger OnSkillActivated = default;
        [SerializeField] BCGameEventTrigger OnTakeDamage = default;

        public void Init(
            CharacterContextFactory c,
            TurnOrderManager turnOrder,
            SkillManager skillManager,
            GameStateManager state,
            CharacterTileManager tilemanager, 
            WorldStateContextFactory w) 
        {
            c.OnContextChanged += OnCharacterContextUpdate;
            m_characterContextFactoryRef = c;
            m_gameContext.Characters = m_characterContextFactoryRef.Context;

            w.OnContextChanged += OnWorldUpdate;
            m_worldContextFactoryRef = w;
            m_gameContext.World = w.Context;

            m_skillManagerRef = skillManager;
            m_skillManagerRef.OnCharacterDefeat += OnCharacterDefeat;

            m_gameStateManagerRef = state;
            m_gameStateManagerRef.OnTurnStart += OnTurnChange;

            m_tileManager = tilemanager;
            turnOrder.OnCharacterTurnStart += SetControllerTarget;

        }

        public void Shutdown() 
        {
            m_characterContextFactoryRef.OnContextChanged -= OnCharacterContextUpdate;
            m_worldContextFactoryRef.OnContextChanged -= OnWorldUpdate;
            m_skillManagerRef.OnCharacterDefeat -= OnCharacterDefeat;
            m_gameStateManagerRef.OnTurnStart -= OnTurnChange;

        }

        public void MovePlayer(Vector3 destination)
        {
            Vector3Int targetCoord = m_gameContext.World.WorldMap.WorldToCell(destination);
            EffectResult result = m_skillManagerRef.BasicMovement(m_gameContext, targetCoord, m_targetId);

            Debug.Log(result.Message);

            if (result.IsSuccessful) 
            {
                UpdateCharacterContext(result.ResultStates.Characters);
                m_worldContextFactoryRef.Update(result.ResultStates.World);
                m_gameStateManagerRef.TransitionToNextState();
            }
            // IMPLEMENT OnSkillFail HERE
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
                // IMPLEMENT OnSkillFail HERE
                return;
            }

            // Prompt Skill target selection here
            string rangeId = skillData.EffectRange;
            int targetLimit = SkillManager.GetSkillTargetLimit(skillId, skillLevel);

            Transform parent = m_gameContext.Characters.MemberDataContainer[m_targetId].PlayerTransform;
            TileId tileId = new TileId(
                CommandTypes.SKILL_ACTIVATE,
                m_targetId.FactionId,
                m_targetId.UnitId);
            m_rangeDisplay.ShowSkillTargetOption(tileId, rangeId, parent.position, targetLimit, parent);
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
            if (result.IsSuccessful) 
            {
                UpdateCharacterContext(result.ResultStates.Characters);
                m_worldContextFactoryRef.Update(result.ResultStates.World);

                Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    {"SkillId", skillId}
                };

                EventInfo eventInfo = new EventInfo(userId, payload);
                OnCharacterSkilllActivate(eventInfo);
            }
            // IMPLEMENT OnSkillFail HERE
        }

        public void OnCharacterDefeat(EventInfo info)
        {
            OnCharacterDefeated?.TriggerEvent(info);
            // [BLOCKOUT] Hide sprite for now
            m_tileManager.HideTile(info.SourceId);
            CharacterClassType defeatedCharacterClass = m_gameContext.
                Characters.MemberDataContainer[info.SourceId].PlayerState.Character.ClassType;

            if (defeatedCharacterClass == CharacterClassType.Master)
            {
                OnLeaderCharacterDefeated(info);
            }
        }

        public void OnLeaderCharacterDefeated(EventInfo info) 
        {
            OnLeaderDefeated?.TriggerEvent(info);
        }

        public void OnCharacterAttacking(EventInfo info)
        {
            OnCharacterAttack?.TriggerEvent(info);
        }

        public void OnCharacterSkilllActivate(EventInfo info)
        {
            OnSkillActivated?.TriggerEvent(info);
        }

        public void OnCharacterTakeDamage(EventInfo info)
        {
            OnTakeDamage?.TriggerEvent(info);
        }

        // When the player selects a member tile on the map, we start controlling the character
        void SetControllerTarget(TileId tileId) 
        {
            m_targetId = tileId;
            m_currentTargetState = m_gameContext.Characters.MemberDataContainer[m_targetId].PlayerState;
        }

        void OnWorldUpdate(WorldStateContext world)
        {
            m_gameContext.World = world;
        }

        void OnCharacterContextUpdate(CharacterContext newContext)
        {
            m_gameContext.Characters = newContext;
        }
        void UpdateCharacterContext(CharacterContext context)
        {
            m_characterContextFactoryRef.Update(context);
        }

        void OnTurnChange() 
        {
            if(m_targetId == null || m_currentTargetState == null) 
            {
                return;
            }
            Transform playerTransform = m_gameContext.Characters.MemberDataContainer[m_targetId].PlayerTransform;

            // This can be modulized to scale with world/character events/states.
            if (m_currentTargetState.CurrentSP < m_currentTargetState.Character.MaxSP) 
            {
                ++m_currentTargetState.CurrentSP;
            }

            CharacterStateContainer newPlayerData = new CharacterStateContainer(
            playerTransform,
            m_currentTargetState);

            m_characterContextFactoryRef.UpdateCharacterData(m_targetId, newPlayerData);
        }


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


