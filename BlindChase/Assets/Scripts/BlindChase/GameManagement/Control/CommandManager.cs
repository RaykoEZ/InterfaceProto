using System;
using System.Collections.Generic;
using BlindChase.Ai;
using BlindChase.Events;


namespace BlindChase.GameManagement
{
    public class CommandManager
    {
        Dictionary<CommandTypes, CharacterCommand> m_playerCommandCollection;

        public void Init(
            BCCharacterController c,
            CommandEventHandler eventHandler,
            NpcManager ai
            ) 
        {
            eventHandler.OnCommand += ExecutePlayerCommand;
            ai.OnNPCCommand += ExecutePlayerCommand;

            // Initialize all available player commands
            m_playerCommandCollection = new Dictionary<CommandTypes, CharacterCommand>
            {
                {CommandTypes.ADVANCE, new MovePlayer(c)},
                {CommandTypes.SKILL_PROMPT, new SkillPrompt(c)},
                {CommandTypes.SKILL_ACTIVATE, new SkillActivate(c)}
            };
        }

        public void Shutdown()
        {
            foreach(CharacterCommand command in m_playerCommandCollection.Values) 
            {
                command.Shutdown();
            }
        }

        void ExecutePlayerCommand(CommandRequestInfo eventArgs) 
        {
            CommandTypes currentCommandType = eventArgs.Command.CommandType;
            CommandArgs args = new CommandArgs(eventArgs.Command.CommandArgs);
            // Prepare commands with additional input args.
            PrepareCommand(currentCommandType, args);
            ExecutCommand_Internal(eventArgs.Command);
        }

        void PrepareCommand(CommandTypes commandType, CommandArgs args) 
        {
            switch (commandType)
            {
                // Let Skill Activation know what skill we are activating.
                case CommandTypes.SKILL_PROMPT:
                    {
                        m_playerCommandCollection[CommandTypes.SKILL_ACTIVATE].ExpectData(args);
                        break;
                    }
                default:
                    break;
            }
        }

        void ExecutCommand_Internal(CommandRequest command, Action OnCommandFinish = null)
        {
            Dictionary<string, object> input = new Dictionary<string, object>();

            CharacterCommand currentCommand = m_playerCommandCollection[command.CommandType];
            CommandArgs args = new CommandArgs(command.CommandArgs);
            currentCommand.ExecuteCommand(args, OnCommandFinish);
        }
    }

}
