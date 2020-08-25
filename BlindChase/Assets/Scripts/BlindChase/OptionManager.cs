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
        CharacterContext m_playerContext = default;
        ControllableTileManager m_playerPieceManagerRef;

        public CommandTypes CurrentCommand { get; private set; } = CommandTypes.NONE;

        public void Init() 
        {
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

        void OnUpdatePlayer(CharacterContext player) 
        {
            m_playerContext = player;
        }


    }

}
