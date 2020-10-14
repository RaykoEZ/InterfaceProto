using System.Collections.Generic;
using BlindChase.GameManagement;
using BlindChase.Events;

namespace BlindChase.Ai
{

    public struct NpcCommandResult 
    {
        public CommandRequest ChosenCommand;
        public CommandResult CommandResult; 
        public NpcCommandResult(CommandRequest newCommand, CommandResult result) 
        {
            ChosenCommand = newCommand;
            CommandResult = result;
        }
    }   

}


