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
        FactionContext m_playerContext = default;
        ControllableTileManager m_playerPieceManagerRef;

        public CommandTypes CurrentCommand { get; private set; } = CommandTypes.NONE;

        public void Init(
            ControllableTileManager tileManager, 
            FactionContextFactory p
            ) 
        {
            m_playerPieceManagerRef = tileManager;
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

        void OnUpdatePlayer(FactionContext player) 
        {
            m_playerContext = player;
        }


    }

}
