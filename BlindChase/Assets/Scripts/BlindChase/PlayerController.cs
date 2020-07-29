using UnityEngine;
using BlindChase.Events;

namespace BlindChase
{

    public class PlayerController
    {
        OptionManager m_optionManagerRef;
        PlayableTileManager m_tileManager;
        WorldContextFactory m_world = default;
        PlayerContextFactory m_player = default;
        GameEventHandler m_eventHandle = null;
        WorldContext m_worldContext = default;
        PlayerContext m_playerContext = default;

        public void Init(PlayableTileManager tilemanager, OptionManager options, GameEventHandler e, WorldContextFactory w, PlayerContextFactory p) 
        {
            m_tileManager = tilemanager;
            m_optionManagerRef = options;
            m_world = w;
            m_player = p;
            m_eventHandle = e;
            //m_eventHandle.GameTileEventTriggered += ExecuteCommand;
            m_tileManager.OnTileEvent += ExecutePlayerCommand;
            OnUpdateWorld(w.Context);
            OnUpdatePlayer(p.Context);
            m_world.SubscribeToContextUpdate(OnUpdateWorld);
            m_player.SubscribeToContextUpdate(OnUpdatePlayer);
        }

        public void Shutdown()
        {
            //m_eventHandle.GameTileEventTriggered -= ExecuteCommand;
            m_world.UnsubscribeToContextUpdate(OnUpdateWorld);
            m_player.UnsubscribeToContextUpdate(OnUpdatePlayer);
        }

        void ExecutePlayerCommand(TileEventInfo eventArgs) 
        {
            switch (m_optionManagerRef.CurrentCommand)
            {
                case CommandTypes.MOVE:
                    {
                        Debug.Log("Move");
                        MovePlayer(eventArgs.Location);
                        break;
                    }
                case CommandTypes.ATTACK:
                    break;
                case CommandTypes.SKILL:
                    break;
                case CommandTypes.EXPLORE:
                    break;
                case CommandTypes.END:
                    break;
                case CommandTypes.NONE:
                    break;
                default:
                    break;
            }
        }

        void MovePlayer(Vector3 destination) 
        {
            Vector3Int destCoord = m_worldContext.WorldMap.WorldToCell(destination);

            Vector3 offset = destCoord - m_playerContext.PlayerCoord;

            m_tileManager.MoveTile(TileDisplayKeywords.PLAYER, offset);

            PlayerContext newContext = new PlayerContext(destCoord, m_tileManager.Player(TileDisplayKeywords.PLAYER).transform);

            m_player.Update(newContext);
        }

        void OnUpdateWorld(WorldContext world)
        {
            m_worldContext = world;
        }

        void OnUpdatePlayer(PlayerContext player)
        {
            m_playerContext = player;
        }
    }

}
