using BlindChase.Utility;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase
{
    // For spawning a player
    public class BlindChaseCore : MonoBehaviour
    {
        [SerializeField] GameObject m_playerAsset = default;
        [SerializeField] SentientTileSpawner m_spawner = default;
        [SerializeField] Tilemap m_map = default;

        WorldContextFactory m_worldContextFactory = new WorldContextFactory();
        PlayerContextFactory m_playerContextFactory = new PlayerContextFactory();

        WorldContext m_worldContext = default;
        PlayerContext m_playerContext = default;

        CommandManager m_commandManager = new CommandManager();

        BCEventHandler m_eventHandler = default;

        void Start()
        {
            SetupEventHandler();

            m_worldContext = m_worldContextFactory.Init(m_map);

            Vector3 p = m_map.GetCellCenterLocal(Vector3Int.zero);

            GameObject o = m_spawner.SpawnTile(
                m_playerAsset, p, 
                GetComponent<CanvasManager>().GameBoardCanvas.transform);
            
            Vector3Int coord = m_map.LocalToCell(o.transform.position);
            m_playerContext = m_playerContextFactory.Init(coord, o.transform);

            GetComponent<OptionManager>().Init(m_worldContextFactory, m_playerContextFactory);
            m_commandManager.Init(m_eventHandler, m_worldContextFactory, m_playerContextFactory);
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        void Shutdown() 
        {
            ShutdownEventHandler();
        }

        void SetupEventHandler() 
        {
            m_eventHandler = new BCEventHandler();
            m_eventHandler.PlayerMoved += PlayerPositionUpdated;
            m_eventHandler.PlayerRefChanged += PlayerObjectUpdated;
            m_eventHandler.WorldChanged += WorldUpdated;

        }

        void ShutdownEventHandler() 
        {
            m_eventHandler.PlayerMoved -= PlayerPositionUpdated;
            m_eventHandler.PlayerRefChanged -= PlayerObjectUpdated;
            m_eventHandler.WorldChanged -= WorldUpdated;
        }

        static void PlayerPositionUpdated(object sender, PlayerPositionEventArgs args)
        {

        }

        static void PlayerObjectUpdated(object sender, PlayerObjectEventArgs args)
        {

        }

        static void WorldUpdated(object sender, WorldEventArgs world)
        {

        }
    }
}