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
    public class PlayerController
    {
        CharacterTileManager m_tileManager;
        GameContextCollection m_gameContext = new GameContextCollection();
        // The id of the faction member tile object we are controlling.
        TileId m_targetId;
        CharacterBehaviour m_playerBehaviourRef;
        CharacterState m_currentTargetState;
        WorldStateContextFactory m_worldContextFactoryRef;
        CharacterContextFactory m_characterContextFactoryRef;
        GameStateManager m_gameStateManagerRef;
        SkillManager m_skillManagerRef;

        CombatHandler m_combatHandler = new CombatHandler();
        public event OnCharacterDefeated OnCharacterDefeated = default;

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
            m_gameStateManagerRef = state;
            m_gameStateManagerRef.OnTurnStart += OnTurnChange;

            m_tileManager = tilemanager;
            turnOrder.OnCharacterTurnStart += SetControllerTarget;
            m_combatHandler.OnCharacterDefeat += OnCharacterDefeat;
        }

        void OnCharacterDefeat(TileId id) 
        {
            OnCharacterDefeated?.Invoke(id);
        }

        // When the player selects a member tile on the map, we start controlling the character
        void SetControllerTarget(TileId tileId) 
        {
            m_targetId = tileId;
            m_currentTargetState = m_gameContext.Characters.MemberDataContainer[m_targetId].PlayerState;
            m_playerBehaviourRef = m_gameContext.Characters.MemberDataContainer[m_targetId].PlayerTransform.
                GetComponent<CharacterBehaviour>();
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

        void UpdateCharacterContext(TileId id)
        {
            CharacterStateContainer stateContainer =
                new CharacterStateContainer(
                    m_gameContext.Characters.MemberDataContainer[id].PlayerTransform,
                    m_gameContext.Characters.MemberDataContainer[id].PlayerState);

            m_characterContextFactoryRef.UpdateCharacterData(id, stateContainer);
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

        public void MovePlayer(Vector3 destination, Vector3 origin)
        {
            m_combatHandler.SetupContext(m_gameContext.World, m_gameContext.Characters, m_tileManager);
            GameContextCollection resultingContext = m_combatHandler.MoveTarget(m_targetId, m_targetId, destination, origin);

            UpdateCharacterContext(resultingContext.Characters);
            m_worldContextFactoryRef.Update(resultingContext.World);
            m_gameStateManagerRef.TransitionToNextState();
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
                return;
            }

            // Prompt Skill target selection here
            string rangeId = skillData.EffectRange;
            int targetLimit = m_skillManagerRef.GetSkillTargetLimit(skillId, skillLevel);
            m_playerBehaviourRef.OnPlayerSkillSelect(rangeId, targetLimit);
        }

        public void ActivateSkill(int skillId, int skillLevel, HashSet<Vector3> targetPos, Vector3 userPos)
        {
            HashSet<TileId> targetCoords = new HashSet<TileId>();
            foreach(Vector3 pos in targetPos)
            {
                Vector3Int coord = m_gameContext.World.WorldMap.WorldToCell(pos);
                TileId target = m_gameContext.World.GetOccupyingTileAt(coord);
                
                if( target!= null) 
                {
                    targetCoords.Add(target);
                }
            }

            Vector3Int userCoord = m_gameContext.World.WorldMap.WorldToCell(userPos);
            TileId userId = m_gameContext.World.GetOccupyingTileAt(userCoord);
            GameContextCollection context = new GameContextCollection 
            { 
                Characters = m_gameContext.Characters, 
                World = m_gameContext.World
            };

            GameContextCollection newContext = m_skillManagerRef.ActivateSkill(skillId, skillLevel, context, targetCoords, userId);

            UpdateCharacterContext(newContext.Characters);
            m_worldContextFactoryRef.Update(newContext.World);
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


