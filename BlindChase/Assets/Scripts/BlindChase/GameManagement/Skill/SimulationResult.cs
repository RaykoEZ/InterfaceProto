using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public struct SimulationResult
    {
        public string Message;
        public GameContextRecord ResultContext;
        public List<CharacterState> AffectedCharacters;
        public List<CharacterState> DefeatedCharacters;

        public SimulationResult(string message, GameContextRecord result, List<CharacterState> affected) 
        {
            Message = message;
            ResultContext = result;
            AffectedCharacters = affected;
            WorldContext world = ResultContext.WorldRecord;
            DefeatedCharacters = new List<CharacterState>();
            // If a character is defeated, remove them from the board.
            foreach ( CharacterState target in affected) 
            {
                if (target.IsDefeated) 
                {
                    Debug.Log($"{target.Character.Name} {target.ObjectId.NPCId} defeated");
                    DefeatedCharacters.Add(target);
                    world.RemoveBoardPiece(target.Position, target.ObjectId);
                }
            }
        }
    }
}


