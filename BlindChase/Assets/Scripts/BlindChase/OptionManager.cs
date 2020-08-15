using System;
using System.Collections;
using UnityEngine;

namespace BlindChase 
{
    public enum CommandTypes 
    { 
        MOVE,
        SKILL,
        END,
        NONE
    }

    public class OptionManager : MonoBehaviour
    {
        WorldContext m_worldContext = default;
        ControllableTileContext m_playerContext = default;
        ControllableTileManager m_playerPieceManagerRef;

        public CommandTypes CurrentCommand { get; private set; } = CommandTypes.NONE;

        public void Init(
            ControllableTileManager tileManager, 
            WorldContextFactory w,
            FactionContextFactory p
            ) 
        {
            m_playerPieceManagerRef = tileManager;

            w.SubscribeToContextUpdate(OnUpdateWorld);
            p.SubscribeToContextUpdate(OnUpdatePlayer);
        }

        public void Shutdown() 
        {
        }

        public void TogglePreviewOption(int command)
        {
            CommandTypes type = (CommandTypes)command;
            CurrentCommand = type;

            switch (type)
            {
                case CommandTypes.MOVE:
                    {                      
                        TileBehaviour playerBehaviour = m_playerPieceManagerRef.Player(m_playerContext.LatestSelectedPieceId);
                        playerBehaviour.OnPlayerSelect();
                        break;
                    }
                case CommandTypes.SKILL:
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

        void OnUpdatePlayer(ControllableTileContext player) 
        {
            m_playerContext = player;
        }


    }

}
