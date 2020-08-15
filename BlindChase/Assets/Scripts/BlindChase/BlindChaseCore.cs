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
        [SerializeField] OptionManager m_optionManager = default;

        WorldContextFactory m_worldContextFactory = new WorldContextFactory();
        FactionContextFactory m_playerContextFactory = new FactionContextFactory();

        PlayerManager m_playerManager = new PlayerManager();
        ControllableTileManager m_playerPieceManager = new ControllableTileManager(); 

        FactionMemberController m_controller = new FactionMemberController();

        GameStateManager m_gameState = new GameStateManager();
        void Start()
        {
            InitGame();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        void InitGame() 
        {
            m_worldContextFactory.Reset(new WorldContext(m_map));

            Stack<GameEffect> startGameEffects = new Stack<GameEffect>();
            List<Faction> teams = new List<Faction>();
            m_gameState.Init(startGameEffects);

            m_playerPieceManager.Init();
            m_worldContextFactory.Reset(new WorldContext(m_map));

            GetComponent<OptionManager>().Init(m_playerPieceManager, m_worldContextFactory, m_playerContextFactory);

            // No players are selected right now
            m_controller.Init(m_playerPieceManager, m_playerContextFactory);

            m_playerManager.Init(
                m_playerPieceManager, 
                m_optionManager,
                m_gameState,
                m_controller,
                m_worldContextFactory, m_playerContextFactory);

            InitPlayer();

        } 

        void InitPlayer() 
        {

            // Spawn a player tile
            Vector3 p = m_map.GetCellCenterLocal(Vector3Int.zero);

            Transform t = GetComponent<CanvasManager>().GameBoardCanvas.transform;

            TileId playerTileId = new TileId(TileDisplayKeywords.PLAYER, "0", "0");

            GameObject playerObject = m_playerPieceManager.SpawnTile(
                playerTileId,
                m_playerAsset, 
                p,
                t
                );

            Vector3Int coord = m_map.LocalToCell(playerObject.transform.position);

            ControllableTileContextData playerData = new ControllableTileContextData(coord, playerObject.transform);

            m_playerContextFactory.UpdateContext(playerTileId, playerData);

            m_playerPieceManager.ShowTile(playerTileId);
        }

        void Shutdown() 
        {
            m_worldContextFactory.Shutdown();
            m_playerContextFactory.Shutdown();
            m_playerManager.Shutdown();
            m_playerPieceManager.Shutdown();
        }

    }
}