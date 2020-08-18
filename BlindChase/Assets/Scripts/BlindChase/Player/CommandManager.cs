using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.State;

namespace BlindChase
{

    public class CommandManager
    {
        GameStateManager m_gameState = default;

        Dictionary<CommandTypes, PlayerCommand> m_playerCommandCollection;

        public void Init(
            ControllableTileManager tilemanager, 
            GameStateManager state,
            FactionMemberController c) 
        {
            m_gameState = state;

            // Initialize all available player commands
            m_playerCommandCollection = new Dictionary<CommandTypes, PlayerCommand>
            {
                {CommandTypes.MOVE, new MovePlayer(c)}
            };

            tilemanager.OnTileEvent += ExecutePlayerCommand;
        }

        public void Shutdown()
        {
        }

        void ExecutePlayerCommand(TileEventInfo eventArgs) 
        {
            Dictionary<string, object> input = new Dictionary<string, object>();

            if (eventArgs.CommandType == CommandTypes.NONE) 
            {
                return;
            }

            CommandTypes currentCommand = eventArgs.CommandType;
            switch (currentCommand)
            {
                case CommandTypes.MOVE:
                    {
                        input.Add("destination", eventArgs.Location);
                        Vector3 origin = (Vector3)eventArgs.Payload["origin"];
                        input.Add("origin", origin);
                        break;
                    }
                case CommandTypes.SKILL:
                    break;
                case CommandTypes.END:
                    // When implement more pieces, check if all pieces are done, warn player.
                    Stack<GameEffect> gameEffects = new Stack<GameEffect>();
                    m_gameState.TransitionToNextState(gameEffects);
                    return;
                default:
                    break;
            }

            CommandArgs args = new CommandArgs(input);
            PlayerCommand command = m_playerCommandCollection[currentCommand];
            command.ExecuteCommand(args);
        }
    }

}
