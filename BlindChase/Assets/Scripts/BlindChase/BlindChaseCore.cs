using System.Collections.Generic;
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

        GameStateContextFactory m_gameStateContextFactory = new GameStateContextFactory();
        ControllableTileContextFactory m_characterContextFactory = new ControllableTileContextFactory();

        CommandManager m_commandManager = new CommandManager();
        ControllableTileManager m_unitTileManager = new ControllableTileManager();

        DeploymentManager m_deploymentManager = new DeploymentManager();

        TileController m_controller = new TileController();
        
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
            m_deploymentManager.Init("test");
            UnitDeploymentList deployment = m_deploymentManager.GetNPCDeployment();

            List<GameEffect> startGameEffects = new List<GameEffect>();
            m_gameState.Init(startGameEffects);

            m_unitTileManager.Init();

            m_controller.Init(
                m_characterContextFactory, 
                m_unitTileManager, 
                m_gameStateContextFactory);

            DeployFactionUnits(deployment.DeploymentInfo);

            m_commandManager.Init(
                m_unitTileManager,
                m_turnOrderManager,
                m_gameState,
                m_controller);


            // We decide on which faction to go first.



            m_gameStateContextFactory.Update(new GameStateContext(m_map));
        }

        void DeployFactionUnits(List<DeploymentInfo> factionDeployment)
        {
            Transform t = GetComponent<CanvasManager>().GameBoardCanvas.transform;
            // For each faction, deploy their units.

            m_deploymentManager.DeployUnits(
                    t, 
                    factionDeployment,
                    m_playerAsset, 
                    m_map,
                    m_unitTileManager,
                    m_characterContextFactory);

            m_turnOrderManager = new TurnOrderManager(m_gameState, m_characterContextFactory);
        }

        void Shutdown() 
        {
            m_gameStateContextFactory.Shutdown();
            m_characterContextFactory.Shutdown();

            m_commandManager.Shutdown();
            m_unitTileManager.Shutdown();
        }

    }
}