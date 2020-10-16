using System.Collections.Generic;

namespace BlindChase.GameManagement
{
    public struct CommandResult
    {
        public string Message;
        public GameContextRecord ResulContext;
        public List<CharacterState> AffectedCharacters;

        public CommandResult(string message, GameContextRecord result, List<CharacterState> affected) 
        {
            Message = message;
            ResulContext = result;
            AffectedCharacters = affected;
        }
    }
}


