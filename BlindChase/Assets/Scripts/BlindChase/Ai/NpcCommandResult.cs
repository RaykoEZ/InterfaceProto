using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public partial class CommandSimulator 
    {
        protected class NpcCommandResult 
        {
            public CommandTypes CommandType = CommandTypes.NONE;
            public SimulationResult SimResult;
            public readonly DecisionParameter Parameters;
            public IReadOnlyList<Vector3Int> Targets;
            public SkillActivationInput SkillInput;

            protected NpcCommandResult(
                CommandTypes commandType,
                in SimulationResult result, 
                in DecisionParameter param, 
                in List<Vector3Int> targets,
                in SkillActivationInput input = default) 
            {
                CommandType = commandType;
                SimResult = result;
                SkillInput = input;
                Parameters = param;
                Targets = targets;
            }

            // Overload for singular target locations.
            protected NpcCommandResult(
                CommandTypes commandType,
                in SimulationResult result,
                in DecisionParameter param,
                in Vector3Int target,
                in SkillActivationInput input = default)
            {
                CommandType = commandType;
                SimResult = result;
                SkillInput = input;
                Parameters = param;
                Targets = new List<Vector3Int> { target };
            }

            public static NpcCommandResult Create(
                CommandTypes commandType,
                in ObjectId userId,
                in DecisionParameter defaultDecisionParam,
                in RangeMap visionRange,
                in RangeMap movementRange,
                in SimulationResult simResult,
                in List<Vector3Int> targetLocations,
                in SkillActivationInput input = default) 
            {
                CharacterState userState = simResult.ResulContext.CharacterRecord.MemberDataContainer[userId].PlayerState;
                DecisionParameter newUrgency = DecisionParameter.CalculateNewParameter(defaultDecisionParam, visionRange, movementRange, simResult.ResulContext, userState);
                NpcCommandResult ret = new NpcCommandResult(commandType, simResult, newUrgency, targetLocations, input);
                return ret;
            }

            // Overload for singular target locations.
            public static NpcCommandResult Create(
                CommandTypes commandType,
                in ObjectId userId,
                in DecisionParameter defaultDecisionParam,
                in RangeMap visionRange,
                in RangeMap movementRange,
                in SimulationResult simResult,
                in Vector3Int targetLocation,
                in SkillActivationInput input = default)
            {
                CharacterState userState = simResult.ResulContext.CharacterRecord.MemberDataContainer[userId].PlayerState;
                DecisionParameter newUrgency = DecisionParameter.CalculateNewParameter(defaultDecisionParam, visionRange, movementRange, simResult.ResulContext, userState);             
                NpcCommandResult ret = new NpcCommandResult(commandType, simResult, newUrgency, targetLocation, input);
                return ret;
            }
        }




    }
}