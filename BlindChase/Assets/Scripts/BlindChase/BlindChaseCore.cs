﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;
using BlindChase.Utility;
using BlindChase.State;

namespace BlindChase
{
    // For spawning a player
    public class BlindChaseCore : MonoBehaviour
    {
        [SerializeField] GameObject m_playerAsset = default;
        [SerializeField] Tilemap m_map = default;
        [SerializeField] CharacterHUDManager m_characterHUD = default;
        [SerializeField] RangeDisplay m_rangeDisplay = default;
        [SerializeField] TileSelectionManager m_tileSelector = default;

        WorldStateContextFactory m_worldStateContextFactory = new WorldStateContextFactory();
        CharacterContextFactory m_characterContextFactory = new CharacterContextFactory();

        SkillManager m_skillManager = default;
        CommandManager m_commandManager = new CommandManager();
        CharacterTileManager m_characterTileManager = new CharacterTileManager();

        CharacterDeploymentManager m_deploymentManager = new CharacterDeploymentManager();

        PlayerController m_controller = new PlayerController();
        
        GameStateManager m_gameState = new GameStateManager();
        TurnOrderManager m_turnOrderManager = default;

        void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        void Init() 
        {
            m_worldStateContextFactory.Update(new WorldStateContext(m_map));

            m_deploymentManager.Init("test");
            CharacterDeploymentList deployment = m_deploymentManager.GetNPCDeployment();
            List<CharacterState> stateList = InitCharacterDeployment(deployment.DeploymentInfo);
            m_turnOrderManager = new TurnOrderManager(m_gameState, m_characterContextFactory);
            m_rangeDisplay.Init(m_turnOrderManager);
            m_characterTileManager.Init(m_turnOrderManager);
            m_characterHUD.Init(m_turnOrderManager, m_characterContextFactory);

            // Append all unique skill ids for characters
            HashSet<int> skillIds = new HashSet<int>();
            foreach (CharacterState characterState in stateList)
            {
                skillIds.UnionWith(characterState.Character.SkillIds);
            }
            // Load all possible skills the deployed characters have into the skill manager.
            m_skillManager = new SkillManager(skillIds);
            m_tileSelector.Init(m_worldStateContextFactory, m_characterTileManager, m_turnOrderManager);

            m_controller.Init(
                m_characterContextFactory,
                m_turnOrderManager,
                m_skillManager,
                m_gameState,
                m_characterTileManager,
                m_worldStateContextFactory);

            m_commandManager.Init(
                m_characterTileManager,
                m_characterHUD,
                m_controller);

            // We decide on which faction to go first.



            List<GameEffect> startGameEffects = new List<GameEffect>();
            m_gameState.StartGame(startGameEffects);

        }

        List<CharacterState> InitCharacterDeployment(List<FactionDeploymentInfo> factionDeployment)
        {
            Transform t = GetComponent<CanvasManager>().GameBoardCanvas.transform;
            // For each faction, deploy their units.

            List<CharacterState> stateList = m_deploymentManager.DeployUnits(
                    t, 
                    factionDeployment,
                    m_playerAsset, 
                    m_map,
                    m_characterTileManager,
                    m_worldStateContextFactory,
                    m_characterContextFactory,
                    m_rangeDisplay);

            return stateList;
        }

        void Shutdown() 
        {
            m_worldStateContextFactory.Shutdown();
            m_characterContextFactory.Shutdown();
            m_rangeDisplay.Shutdown();
            m_commandManager.Shutdown();
            m_characterTileManager.Shutdown();
        }

    }
}