using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.State;

namespace BlindChase
{

    public class CommandManager
    {

        Dictionary<CommandTypes, PlayerCommand> m_playerCommandCollection;

        public void Init(
            CharacterTileManager tilemanager,
            CharacterHUDManager characterHUD,
            PlayerController c) 
        {
            tilemanager.OnTileCommand += ExecutePlayerCommand;
            characterHUD.OnPlayerCommand += ExecutePlayerCommand;

            // Initialize all available player commands
            m_playerCommandCollection = new Dictionary<CommandTypes, PlayerCommand>
            {
                {CommandTypes.MOVE, new MovePlayer(c)},
                {CommandTypes.SKILL_PROMPT, new SkillPrompt(c)},
                {CommandTypes.SKILL_ACTIVATE, new SkillActivate(c)}
            };

        }

        public void Shutdown()
        {
        }

        void ExecutePlayerCommand(CommandEventInfo eventArgs) 
        {
            Dictionary<string, object> input = new Dictionary<string, object>();

            CommandTypes currentCommand = eventArgs.CommandType;
            CommandArgs args = new CommandArgs(input);

            switch (currentCommand)
            {
                case CommandTypes.MOVE:
                    {
                        input.Add("Destination", eventArgs.Location);
                        Vector3 origin = (Vector3)eventArgs.Payload["Origin"];
                        input.Add("Origin", origin);
                        break;
                    }
                case CommandTypes.SKILL_PROMPT:
                    {
                        input.Add("SkillId", eventArgs.Payload["SkillId"]);
                        input.Add("SkillLevel", eventArgs.Payload["SkillLevel"]);
                        m_playerCommandCollection[CommandTypes.SKILL_ACTIVATE].ExpectData(args);
                        break;
                    }
                case CommandTypes.SKILL_ACTIVATE:
                    {
                        input.Add("Target", eventArgs.Payload["Target"]);
                        input.Add("Origin", (Vector3)eventArgs.Payload["Origin"]);
                        break;
                    }
                default:
                    break;
            }

            PlayerCommand command = m_playerCommandCollection[currentCommand];
            command.ExecuteCommand(args);
        }
    }

}
