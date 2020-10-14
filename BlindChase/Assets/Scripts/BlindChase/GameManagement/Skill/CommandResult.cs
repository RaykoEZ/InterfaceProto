using System.Collections.Generic;

namespace BlindChase.GameManagement
{
    public struct CommandResult
    {
        public bool IsSuccessful;
        public string Message;
        public GameContextRecord ResulContext;
        public List<CharacterState> AffectedCharacters;

        public void OnSuccess(string message, GameContextRecord result, List<CharacterState> affected) 
        {
            IsSuccessful = true;
            Message = message;
            ResulContext = result;
            AffectedCharacters = affected;
        }

        public void OnFail(string message)
        {
            IsSuccessful = false;
            Message = message;
        }
    }
}


