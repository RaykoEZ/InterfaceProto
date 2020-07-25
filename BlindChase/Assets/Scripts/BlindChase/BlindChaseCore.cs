using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;
using BlindChase.Utility;

namespace BlindChase
{
    // For spawning a player
    public class BlindChaseCore : MonoBehaviour
    {
        [SerializeField] GameObject m_playerAsset = default;
        [SerializeField] Tilemap m_map = default;
        [SerializeField] OptionManager m_optionManager = default;

        WorldContextFactory m_worldContextFactory = new WorldContextFactory();
        PlayerContextFactory m_playerContextFactory = new PlayerContextFactory();

        PlayerController m_playerContoller = new PlayerController();
        PlayableTileManager m_playerTileManager = new PlayableTileManager(); 

        GameEventHandler m_eventHandler = default;

        OnPlayerUpdate OnPlayerUpdate { get; set; }
        OnWorldUpdate OnWorldUpdate { get; set; }

        void Start()
        {
            m_playerTileManager.Init();

            SetupEventHandler();

            m_worldContextFactory.SubscribeToContextUpdate(OnNewWorldContext);
            m_worldContextFactory.Update(new WorldContext(m_map));

            InitPlayer();

            GetComponent<OptionManager>().Init(m_worldContextFactory.Context, m_playerContextFactory.Context, m_playerTileManager, OnPlayerUpdate, OnWorldUpdate);
            m_playerContoller.Init(m_playerTileManager, m_optionManager, m_eventHandler, m_worldContextFactory, m_playerContextFactory);
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        void InitPlayer() 
        {
            // Spawn
            Vector3 p = m_map.GetCellCenterLocal(Vector3Int.zero);

            Transform t = GetComponent<CanvasManager>().GameBoardCanvas.transform;

            GameObject o = m_playerTileManager.SpawnTile(
                TileDisplayKeywords.PLAYER,
                m_playerAsset, 
                p,
                t
                );

            m_playerTileManager.ShowTile(TileDisplayKeywords.PLAYER);
            GameObject player = o;

            Vector3Int coord = m_map.LocalToCell(player.transform.position);
            m_playerContextFactory.SubscribeToContextUpdate(OnNewPlayerContext);
            m_playerContextFactory.Update(new PlayerContext(coord, o.transform));

        }

        void Shutdown() 
        {
            m_playerContoller.Shutdown();
            m_playerTileManager.Shutdown();
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
            m_eventHandler = new GameEventHandler();
            m_eventHandler.Init(m_playerTileManager);
        }

    }
}