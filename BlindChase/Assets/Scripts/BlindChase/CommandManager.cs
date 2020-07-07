using UnityEngine;
using BlindChase.Events;

namespace BlindChase
{

    public class CommandManager
    {
        WorldContextFactory m_world = default;
        PlayerContextFactory m_player = default;
        BCEventHandler m_eventHandle = null;
        WorldContext m_worldContext = default;
        PlayerContext m_playerContext = default;


        public void Init(BCEventHandler e, WorldContextFactory w, PlayerContextFactory p) 
        {
            m_world = w;
            m_player = p;
            m_eventHandle = e;
            m_eventHandle.GameEventTriggered += ExecuteCommand;
            OnUpdateWorld(w.Context);
            OnUpdatePlayer(p.Context);
            m_world.SubscribeToContextUpdate(OnUpdateWorld);
            m_player.SubscribeToContextUpdate(OnUpdatePlayer);
        }

        public void Shutdown()
        {
            m_eventHandle.GameEventTriggered -= ExecuteCommand;
            m_world.UnsubscribeToContextUpdate(OnUpdateWorld);
            m_player.UnsubscribeToContextUpdate(OnUpdatePlayer);
        }

        void ExecuteCommand(object sender, BCEventArgs eventArgs) 
        {
            switch (eventArgs.CommandType)
            {
                case CommandTypes.MOVE:
                    {
                        Debug.Log("Move");
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
