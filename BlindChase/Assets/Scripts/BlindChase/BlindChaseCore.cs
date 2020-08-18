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

        WorldContextFactory m_worldContextFactory = new WorldContextFactory();
        List<FactionContextFactory> m_factionContextFactories = new List<FactionContextFactory>();

        CommandManager m_playerManager = new CommandManager();
        ControllableTileManager m_playerTileManager = new ControllableTileManager(); 

        FactionMemberController m_controller = new FactionMemberController();
        FactionManager m_factionManager = default;

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
            Stack<GameEffect> startGameEffects = new Stack<GameEffect>();
            m_gameState.Init(startGameEffects);
            m_playerTileManager.Init();

            List<string> factionIds = new List<string> 
            {
                {"test1"},
                {"test2"}
            };

            for (int i = 0; i < factionIds.Count; ++i) 
            {
                m_factionContextFactories.Add(new FactionContextFactory(factionIds[i]));
            }

            GetComponent<OptionManager>().Init(m_playerTileManager, m_factionContextFactories[0]);

            m_factionManager = new FactionManager(m_factionContextFactories);
            m_controller.Init(m_factionManager, m_playerTileManager, m_worldContextFactory);

            m_playerManager.Init(
                m_playerTileManager, 
                m_gameState,
                m_controller);

            InitPlayer(factionIds);

            m_worldContextFactory.Reset(new WorldContext(m_map));

        }

        void InitPlayer(List<string> factionIds)
        {
            Transform t = GetComponent<CanvasManager>().GameBoardCanvas.transform;

            for (int i = 0; i < factionIds.Count; ++i) 
            {
                TileId playerId = new TileId(TileDisplayKeywords.PLAYER, factionIds[i], i.ToString());
                Vector3 p = m_map.GetCellCenterLocal(new Vector3Int(0, i, 0));

                GameObject playerObject = m_playerTileManager.SpawnTile(
                    playerId,
                    m_playerAsset,
                    p,
                    t
                );

                Vector3Int coord = m_map.LocalToCell(playerObject.transform.position);

                ControllableDataContainer playerData = new ControllableDataContainer(coord, playerObject.transform);

                foreach (FactionContextFactory factory in m_factionContextFactories)
                {
                    factory.UpdateContext(playerId, playerData);
                }

                m_playerTileManager.ShowTile(playerId);
            }
        }

        void Shutdown() 
        {
            m_worldContextFactory.Shutdown();

            foreach(FactionContextFactory faction in m_factionContextFactories) 
            {
                faction.Shutdown();
            }

            m_playerManager.Shutdown();
            m_playerTileManager.Shutdown();
        }

    }
}