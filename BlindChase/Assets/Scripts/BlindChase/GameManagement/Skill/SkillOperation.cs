using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public partial class SkillEffect
    {
        // This contains all possible calls available for a skill effect.
        static partial class SkillOperation
        {
            public static SimulationResult AutoRecovery(SkillEffectArgs args) 
            {
                // always recover self
                ObjectId target = args.TargetId;
                GameContextRecord newContext = ActionSimulation.RestoreSP(target, 1, args.Context);
                List<int> skillIds = new List<int>(args.TargetState.CurrentSkillCooldowns.Keys);
                newContext = ActionSimulation.SkillCDReduction(target, 1, skillIds, newContext);

                CharacterState newState = newContext.CharacterRecord.MemberDataContainer[target].PlayerState;
                List<CharacterState> changeList = new List<CharacterState>{ { newState } };
                SimulationResult result = new SimulationResult("Recover SP and cooldown.", newContext, changeList);

                return result;
            }

            public static SimulationResult Strike(SkillEffectArgs args) 
            {
                ObjectId target = args.TargetId;
                int baseValue = args.SkillData.BaseValue;
                GameContextRecord newContext = ActionSimulation.DealDamage(target, baseValue, args.Context);

                CharacterState newState = newContext.CharacterRecord.MemberDataContainer[target].PlayerState;
                List<CharacterState> changeList = new List<CharacterState> { { args.UserState }, { newState } };
                SimulationResult result = new SimulationResult("Strike Activated", newContext, changeList);       
                return result;
            }

            public static SimulationResult FirstAid(SkillEffectArgs args) 
            {
                ObjectId target = args.TargetId;
                int baseValue = args.SkillData.BaseValue;
                GameContextRecord newContext = ActionSimulation.RestoreHP(target, baseValue, args.Context);
                CharacterState newState = newContext.CharacterRecord.MemberDataContainer[target].PlayerState;

                List<CharacterState> changeList = new List<CharacterState> { { args.UserState }, { newState } };
                CharacterState oldTarget = args.TargetState;
                string message = oldTarget.CurrentHP >= oldTarget.Character.MaxHP ? "Target is at full HP." : "First Aid activated";
                SimulationResult result = new SimulationResult(message, newContext, changeList);
                return result;
            }

            public static SimulationResult BasicMovement(SkillEffectArgs args) 
            {
                CharacterState userState = args.UserState;

                List<CharacterState> changeList = new List<CharacterState>(1);
                GameContextRecord gameContext;
                ObjectId occupier = args.TargetId;
                bool isFreeSpace = occupier == null;
                string message;
                Vector3Int dest = args.TargetCoord;
                // If no one is occupying the destination, move as usual
                // Else, enter combat
                if (isFreeSpace) 
                {
                    gameContext = ActionSimulation.MoveTo(
                        args.Context,
                        args.UserId,
                        dest,
                        userState.Position);
                    message = $"Moving character to {dest}";
                    changeList.Add(gameContext.CharacterRecord.MemberDataContainer[args.UserId].PlayerState);
                }
                else
                {
                    // Check for an enemy defender at the would-be knockback location, 
                    // if there is a defender at that position, this skill fails.
                    gameContext = ActionSimulation.Combat(
                        args.Context,
                        args.UserId,
                        occupier);
                    message = $"Attacking enemy at {dest}";
                    CharacterContext c = gameContext.CharacterRecord;
                    changeList.Add(c.MemberDataContainer[args.UserId].PlayerState);
                    changeList.Add(c.MemberDataContainer[occupier].PlayerState);
                }

                SimulationResult result = new SimulationResult(message, gameContext, changeList);
                return result;
            }
        }
    }
}


