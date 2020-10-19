using System.Collections.Generic;

namespace BlindChase.GameManagement
{
    public struct SimulationResult
    {
        public string Message;
        public GameContextRecord ResulContext;
        public List<CharacterState> AffectedCharacters;

        public SimulationResult(string message, GameContextRecord result, List<CharacterState> affected) 
        {
            Message = message;
            ResulContext = result;
            AffectedCharacters = affected;
        }
    }
}


