using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.State;

namespace BlindChase
{

    public class PlayerManager
    {
        OptionManager m_optionManagerRef;
        GameStateManager m_gameState = default;

        WorldContext m_worldContext = default;
        ControllableTileContext m_playerContext = default;

        Dictionary<CommandTypes, PlayerCommand> m_playerCommandCollection;

        public void Init(
            ControllableTileManager tilemanager, 
            OptionManager options,
            GameStateManager state,
            FactionMemberController c, WorldContextFactory w, 
            FactionContextFactory p) 
        {
            m_gameState = state;
            m_optionManagerRef = options;

            // Initialize all available player commands
            m_playerCommandCollection = new Dictionary<CommandTypes, PlayerCommand>
            {
                {CommandTypes.MOVE, new MovePlayer(c)}
            };


            tilemanager.OnTileEvent += ExecutePlayerCommand;

            OnUpdateWorld(w.Context);
            OnUpdatePlayer(p.Context);
            w.SubscribeToContextUpdate(OnUpdateWorld);
            p.SubscribeToContextUpdate(OnUpdatePlayer);
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
                        input.Add("destination", m_worldContext.WorldMap.WorldToCell(eventArgs.Location));
                        Vector3Int origin = m_playerContext.FactionMembers[eventArgs.TileId].PlayerCoord;
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
            // Toggle command preview display off. 
            m_optionManagerRef.TogglePreviewOption((int)currentCommand);
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
