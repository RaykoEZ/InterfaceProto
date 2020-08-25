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
        TileId m_activeCharacterId;

        public void Init(
            ControllableTileManager tilemanager,
            TurnOrderManager turnOrderManager,
            GameStateManager state,
            TileController c) 
        {
            m_gameState = state;
            turnOrderManager.OnCharacterActivate += OnActiveFactionUpdated;
            tilemanager.OnTileEvent += ExecutePlayerCommand;
            m_activeCharacterId = turnOrderManager.GetActiveCharacterId();
            // Initialize all available player commands
            m_playerCommandCollection = new Dictionary<CommandTypes, PlayerCommand>
            {
                {CommandTypes.MOVE, new MovePlayer(c)}
            };

        }

        public void Shutdown()
        {
        }

        void ExecutePlayerCommand(TileEventInfo eventArgs) 
        {
            Dictionary<string, object> input = new Dictionary<string, object>();

            bool isPlayerTurn = m_gameState.CurrentGameState() == typeof(PlayerTurn);
            // If game state hasn't finished processing, don't execute command
            if (!isPlayerTurn)
            {
                return;
            }

            // If selected tile is not the current tile to be moved, don't execute command on that tile
            if (eventArgs.TileId != m_activeCharacterId)
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
                    return;
                default:
                    break;
            }

            CommandArgs args = new CommandArgs(input);
            PlayerCommand command = m_playerCommandCollection[currentCommand];
            m_gameState.TransitionToNextState();
            command.ExecuteCommand(args);
        }

        void OnActiveFactionUpdated(TileId newId) 
        {
            m_activeCharacterId = newId;
        }
    }

}
