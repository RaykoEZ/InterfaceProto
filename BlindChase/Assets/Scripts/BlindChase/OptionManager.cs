using System;
using System.Collections;
using UnityEngine;

namespace BlindChase 
{
    // NOTE: THIS CAN NEED A CLASS LATER
    public enum CommandTypes 
    { 
        MOVE,
        ATTACK,
        SKILL,
        EXPLORE,
        END,
        NONE
    }

    public class OptionManager : MonoBehaviour
    {
        WorldContextFactory m_world = default;
        PlayerContextFactory m_player = default;

        WorldContext m_worldContext = default;
        PlayerContext m_playerContext = default;

        [SerializeField] RangeDisplay rangeDisplay = default;

        public void Init(WorldContextFactory c, PlayerContextFactory p) 
        {
            m_world = c;
            m_player = p;

            OnUpdateWorld(c.Context);
            OnUpdatePlayer(p.Context);
            m_world.SubscribeToContextUpdate(OnUpdateWorld);
            m_player.SubscribeToContextUpdate(OnUpdatePlayer);
        }

        public void Shutdown() 
        {
            m_world.UnsubscribeToContextUpdate(OnUpdateWorld);
            m_player.UnsubscribeToContextUpdate(OnUpdatePlayer);
        }

        public void PreviewOption(int command)
        {
            CommandTypes type = (CommandTypes)command;
            switch (type)
            {
                case CommandTypes.MOVE:
                    {

                        rangeDisplay.ToggleRangeDisplay(m_playerContext.RangeMaps[1], m_playerContext.PlayerCoord, m_worldContext);
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
