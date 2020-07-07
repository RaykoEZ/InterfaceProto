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
        [SerializeField] TileSpawner m_spawner = default;
        [SerializeField] Tilemap m_map = default;

        WorldContextFactory m_worldContextFactory = new WorldContextFactory();
        PlayerContextFactory m_playerContextFactory = new PlayerContextFactory();

        WorldContext m_worldContext = default;
        PlayerContext m_playerContext = default;

        CommandManager m_commandManager = new CommandManager();

        BCEventHandler m_eventHandler = default;

        OnPlayerUpdate OnPlayerUpdate { get; set; }
        OnWorldUpdate OnWorldUpdate { get; set; }

        void Start()
        {
            SetupEventHandler();

            m_worldContext = m_worldContextFactory.Init(m_map, m_eventHandler);
            m_worldContextFactory.SubscribeToContextUpdate(OnNewWorldContext);

            // Spawn
            Vector3 p = m_map.GetCellCenterLocal(Vector3Int.zero);

            GameObject o = m_spawner.SpawnTile(
                m_playerAsset, p, 
                GetComponent<CanvasManager>().GameBoardCanvas.transform,
                m_eventHandler
                );
            
            Vector3Int coord = m_map.LocalToCell(o.transform.position);
            m_playerContext = m_playerContextFactory.Init(coord, o.transform);
            m_playerContextFactory.SubscribeToContextUpdate(OnNewPlayerContext);


            GetComponent<OptionManager>().Init(m_worldContext, m_playerContext, OnPlayerUpdate, OnWorldUpdate);
            m_commandManager.Init(m_eventHandler, m_worldContextFactory, m_playerContextFactory);
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        void Shutdown() 
        {
            ShutdownEventHandler();
            m_commandManager.Shutdown();
            OnPlayerUpdate = null;
            OnWorldUpdate = null;
        }

        void OnNewPlayerContext(PlayerContext p) 
        {
            OnPlayerUpdate?.Invoke(p);
        }

        void OnNewWorldContext(WorldContext w)
        {
            OnWorldUpdate?.Invoke(w);
        }

        void SetupEventHandler() 
        {
            m_eventHandler = new BCEventHandler();
            m_eventHandler.GameEventTriggered += PlayerPositionUpdated;
        }

        void ShutdownEventHandler() 
        {
            m_eventHandler.GameEventTriggered -= PlayerPositionUpdated;
        }

        static void PlayerPositionUpdated(object sender, BCEventArgs args)
        {

        }
    }
}