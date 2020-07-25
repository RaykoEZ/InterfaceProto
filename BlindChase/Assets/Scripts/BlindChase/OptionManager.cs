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
        WorldContext m_worldContext = default;
        PlayerContext m_playerContext = default;
        PlayableTileManager m_playerManagerRef;

        public CommandTypes CurrentCommand { get; private set; } = CommandTypes.NONE;

        public void Init(
            WorldContext c, PlayerContext p,
            PlayableTileManager tileManager,
            OnPlayerUpdate onPlayerUpdate, 
            OnWorldUpdate onWorldUpdate) 
        {
            m_playerManagerRef = tileManager;
            OnUpdateWorld(c);
            OnUpdatePlayer(p);

            onPlayerUpdate += OnUpdatePlayer;
            onWorldUpdate += OnUpdateWorld;
        }

        public void Shutdown() 
        {
        }

        public void PreviewOption(int command)
        {
            CommandTypes type = (CommandTypes)command;
            CurrentCommand = type;

            switch (type)
            {
                case CommandTypes.MOVE:
                    {
                        TileBehaviour playerBehaviour = m_playerManagerRef.Player(TileDisplayKeywords.PLAYER);
                        playerBehaviour.OnPlayerSelect(m_worldContext, m_playerContext);
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
                    CurrentCommand = CommandTypes.NONE;
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
