using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public partial class SkillEffect
    {
        // This contains all possible calls available for a skill effect.
        static partial class SkillOperation
        {
            static CommandResult OnNoTargetSelected() 
            {
                CommandResult result = new CommandResult();
                result.OnFail("No Target(s) found.");
                return result;
            }

            public static CommandResult AutoRecovery(SkillEffectArgs args) 
            {
                // always recover self
                CharacterState target = args.UserState;

                if (target.CurrentSP < target.Character.MaxSP)
                {
                    ++target.CurrentSP;
                }

                List<int> skillsToCooldown = new List<int>();
                foreach(int skillId in target.CurrentSkillCooldowns.Keys) 
                {
                    if (target.CurrentSkillCooldowns[skillId] > 0) 
                    {
                        skillsToCooldown.Add(skillId);
                    }
                }

                foreach(int skillId in skillsToCooldown) 
                {
                    --target.CurrentSkillCooldowns[skillId];
                }

                CommandResult result = new CommandResult();

                List<CharacterState> changeList = new List<CharacterState>{ { target } };
                result.OnSuccess("Recover SP and cooldown.", args.Context, changeList);

                return result;
            }

            public static CommandResult Strike(SkillEffectArgs args) 
            {
                if (args.TargetId == null)
                {
                    return OnNoTargetSelected();
                }

                CommandResult result = new CommandResult();
                CharacterState target = args.TargetState;

                int baseValue = args.SkillData.BaseValue;
                target = ActionSimulation.DealDamage(baseValue, ref target);

                List<CharacterState> changeList = new List<CharacterState> { { args.UserState }, { target } };
                result.OnSuccess("Strike Activated", args.Context, changeList);
                
                return result;
            }

            public static CommandResult FirstAid(SkillEffectArgs args) 
            {
                if (args.TargetId == null)
                {
                    return OnNoTargetSelected();
                }

                CommandResult result = new CommandResult();
                CharacterState target = args.TargetState;
                if (target.CurrentHP >= target.Character.MaxHP) 
                {
                    result.OnFail("Target is at full HP.");
                }
                else 
                {
                    int baseValue = args.SkillData.BaseValue;
                    target = ActionSimulation.RestoreHP(baseValue, ref target);
                    List<CharacterState> changeList = new List<CharacterState> { { target } };
                    result.OnSuccess("First Aid activated", args.Context, changeList);
                }

                return result;
            }

            public static CommandResult BasicMovement(SkillEffectArgs args) 
            {
                Vector3Int dest = args.TargetCoord;
                CharacterState userState = args.UserState;

                ObjectId occupier = args.TargetId;
                CommandResult result = new CommandResult();
                GameContextRecord gameContext;
                // If no one is occupying the destination, move as usual
                // Else, enter combat
                if (occupier == null) 
                {
                    gameContext = ActionSimulation.MoveTo(
                        args.Context,
                        args.UserId,
                        dest,
                        userState.Position);

                    List<CharacterState> changeList = new List<CharacterState> 
                    { 
                        {gameContext.CharacterRecord.MemberDataContainer[args.UserId].PlayerState}
                    };

                    result.OnSuccess($"Moving character to {dest}", gameContext, changeList);
                }
                else if(occupier.FactionId != args.UserId.FactionId)
                {
                    CharacterState targetState = args.Context.CharacterRecord.
                        MemberDataContainer[occupier].PlayerState;

                    Vector3Int retreatDestination = ActionSimulation.GetClassKnockbackPattern(
                        userState.Character.ClassType, userState.Position, targetState.Position);

                    ObjectId defender = args.Context.WorldRecord.GetOccupyingTileAt(retreatDestination);

                    // Check for an enemy defender at the would-be knockback location, 
                    // if there is a defender at that position, this skill fails.
                    if (defender != null && defender.FactionId == occupier.FactionId) 
                    {
                        result.OnFail($"Enemy is reinforced at {dest} by forces at {retreatDestination}.");
                    }
                    // If there is no defender, the skill succeeds.
                    else 
                    {
                        gameContext = ActionSimulation.Combat(
                            args.Context,
                            args.UserId,
                            occupier);

                        List<CharacterState> changeList = new List<CharacterState> 
                        { 
                            { gameContext.CharacterRecord.MemberDataContainer[args.UserId].PlayerState },
                            { gameContext.CharacterRecord.MemberDataContainer[occupier].PlayerState }
                        };

                        result.OnSuccess($"Attacking enemy at {dest}", gameContext, changeList);
                    }

                }
                // Cannot attack ally occupier, reject this activation.
                else 
                {
                    result.OnFail("Cannot attack allied character.");
                }

                return result;
            }
        }
    }
}


