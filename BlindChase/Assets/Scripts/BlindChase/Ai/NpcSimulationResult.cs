using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public partial class DecisionHelper 
    {
        protected class NpcSimulationResult 
        {
            public CommandTypes CommandType = CommandTypes.NONE;
            public SimulationResult SimResult;
            public readonly DecisionParameter Parameters;
            public readonly DecisionParameter Diff;
            public IReadOnlyList<Vector3Int> Targets;
            public SkillActivationInput SkillInput;

            protected NpcSimulationResult(
                CommandTypes commandType,
                in SimulationResult result, 
                in DecisionParameter param, 
                in DecisionParameter diff,
                in List<Vector3Int> targets,
                in SkillActivationInput input = default) 
            {
                CommandType = commandType;
                SimResult = result;
                SkillInput = input;
                Parameters = param;
                Diff = diff;
                Targets = targets;
            }

            // Overload for singular target locations.
            protected NpcSimulationResult(
                CommandTypes commandType,
                in SimulationResult result,
                in DecisionParameter param,
                in DecisionParameter diff,
                in Vector3Int target,
                in SkillActivationInput input = default)
            {
                CommandType = commandType;
                SimResult = result;
                SkillInput = input;
                Parameters = param;
                Diff = diff;
                Targets = new List<Vector3Int> { target };
            }

            public static NpcSimulationResult Create(
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
                DecisionParameter diff = newUrgency.DiffWith(defaultDecisionParam);

                NpcSimulationResult ret = new NpcSimulationResult(commandType, simResult, newUrgency, diff, targetLocations, input);
                return ret;
            }

            // Overload for singular target locations.
            public static NpcSimulationResult Create(
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
                DecisionParameter diff = newUrgency.DiffWith(defaultDecisionParam);
                
                NpcSimulationResult ret = new NpcSimulationResult(commandType, simResult, newUrgency, diff, targetLocation, input);
                return ret;
            }
        }




    }
}