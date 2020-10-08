using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Ai;
using BlindChase.GameManagement;
using BlindChase.Events;
using BlindChase.Utility;
using BlindChase.Ui;
using BlindChase.State;

namespace BlindChase
{
    // For spawning a player
    public class BlindChaseCore : MonoBehaviour
    {
        [SerializeField] GameObject m_playerAsset = default;
        [SerializeField] Tilemap m_map = default;
        [SerializeField] GameplayScreen m_gameplayScreen = default;
        [SerializeField] PromptHandler m_promptHandler = default;
        [SerializeField] TileSelectionManager m_tileSelector = default;
        [SerializeField] BCCharacterController m_controller = default;
        [SerializeField] CharacterManager m_characterTileManager = default;
        [SerializeField] CommandEventHandler m_commandEventHandler = default;
        [SerializeField] TurnOrderManager m_turnOrderManager = default;

        WorldStateContextFactory m_worldStateContextFactory = new WorldStateContextFactory();
        CharacterContextFactory m_characterContextFactory = new CharacterContextFactory();
        
        SkillManager m_skillManager = default;
        NpcManager m_npcManager = new NpcManager();
        CommandManager m_commandManager = new CommandManager();
        CharacterDeploymentManager m_deploymentManager = new CharacterDeploymentManager();
       
        TurnProgressManager m_gameProgression = new TurnProgressManager();

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
            m_worldStateContextFactory.Update(new WorldContext(m_map));

            m_deploymentManager.Init("test");
            CharacterDeploymentList deployment = m_deploymentManager.GetNPCDeployment();
            List<CharacterState> stateList = InitCharacterDeployment(deployment.DeploymentInfo);
            m_turnOrderManager.Init(m_gameProgression, m_characterContextFactory);
            m_promptHandler.Init(m_characterContextFactory, m_turnOrderManager);
            m_characterTileManager.Init();

            // Append all unique skill ids for characters
            HashSet<int> skillIds = new HashSet<int>();
            foreach (CharacterState characterState in stateList)
            {
                skillIds.UnionWith(characterState.Character.SkillIds);
            }
            // Load all possible skills the deployed characters have into the skill manager.
            m_skillManager = new SkillManager(skillIds);
            m_npcManager.Init(m_characterContextFactory, m_worldStateContextFactory, m_skillManager);

            m_tileSelector.Init(m_worldStateContextFactory, m_characterContextFactory, m_turnOrderManager);
            m_gameplayScreen.Init(m_turnOrderManager, m_characterContextFactory);

            m_controller.Init(
                m_characterContextFactory,
                m_worldStateContextFactory,
                m_turnOrderManager,
                m_npcManager,
                m_skillManager,
                m_gameProgression,
                m_characterTileManager
                );


            m_commandManager.Init(
                m_controller,
                m_commandEventHandler,
                m_npcManager
                );

            // We decide on which faction to go first.


            // Let the game begin
            m_gameProgression.StartGame();
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
                    m_promptHandler);

            return stateList;
        }

        void Shutdown() 
        {
            m_worldStateContextFactory.Shutdown();
            m_characterContextFactory.Shutdown();
            m_npcManager.Shutdown();
            m_promptHandler.Shutdown();
            m_commandManager.Shutdown();
            m_gameProgression.Shutdown();
            m_turnOrderManager.Shutdown();
            m_characterTileManager.Shutdown();
            m_controller.Shutdown();
        }

    }
}