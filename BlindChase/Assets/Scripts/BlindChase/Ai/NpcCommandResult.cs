using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public class NpcCommandResult 
    {
        public CommandTypes CommandType = CommandTypes.NONE;
        public SimulationResult SimResult;
        public IReadOnlyList<Vector3Int> Targets;
        public SkillActivationInput SkillInput;

        public NpcCommandResult(
            CommandTypes commandType,
            in SimulationResult result, 
            in IEnumerable<Vector3Int> targets,
            in SkillActivationInput input = default) 
        {
            CommandType = commandType;
            SimResult = result;
            SkillInput = input;
            Targets = new List<Vector3Int>(targets);
        }

        // Overload for singular target locations.
        public NpcCommandResult(
            CommandTypes commandType,
            in SimulationResult result,
            in Vector3Int target,
            in SkillActivationInput input = default)
        {
            CommandType = commandType;
            SimResult = result;
            SkillInput = input;
            Targets = new List<Vector3Int> { target };
        }
    }   
}