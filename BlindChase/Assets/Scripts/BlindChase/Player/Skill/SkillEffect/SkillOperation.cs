using UnityEngine;
namespace BlindChase
{
    public partial class SkillEffect
    {
        // This contains all possible calls available for a skill effect.
        static partial class SkillOperation
        {
            static EffectResult OnNoTargetSelected(SkillEffectArgs args) 
            {
                EffectResult result = new EffectResult();
                result.OnFail("No Target(s) found.", args.Context);
                return result;
            }

            public static EffectResult Strike(SkillEffectArgs args) 
            {
                if (args.TargetId == null)
                {
                    return OnNoTargetSelected(args);
                }

                EffectResult result = new EffectResult();
                CharacterState target = args.Context.Characters.MemberDataContainer[args.TargetId].PlayerState;

                int baseValue = args.SkillData.BaseValue;
                target = SkillUtility.DealDamage(baseValue, ref target);
                result.OnSuccess("Strike Activated", args.Context);
                
                return result;
            }

            public static EffectResult FirstAid(SkillEffectArgs args) 
            {
                if (args.TargetId == null)
                {
                    return OnNoTargetSelected(args);
                }


                EffectResult result = new EffectResult();
                CharacterState target = args.Context.Characters.MemberDataContainer[args.TargetId].PlayerState;
                if (target.CurrentHP >= target.Character.MaxHP) 
                {
                    result.OnFail("Target is at full HP.", args.Context);
                }
                else 
                {
                    int baseValue = args.SkillData.BaseValue;
                    target = SkillUtility.RestoreHP(baseValue, ref target);
                    result.OnSuccess("First Aid activated", args.Context);
                }

                return result;
            }

            public static EffectResult BasicMovement(SkillEffectArgs args) 
            {
                Vector3Int dest = args.TargetCoord;
                CharacterState userState = args.Context.Characters.
                    MemberDataContainer[args.UserId].PlayerState;

                TileId occupier = args.TargetId;
                EffectResult result = new EffectResult();
                GameContextCollection gameContext;
                // If no one is occupying the destination, move as usual
                // Else, enter combat
                if (occupier == null) 
                {
                    gameContext = SkillUtility.MoveTo(
                        args.Context,
                        args.UserId,
                        dest,
                        userState.Position);

                    result.OnSuccess($"Moving character to {dest}", gameContext);
                }
                else if(occupier.FactionId != args.UserId.FactionId)
                {
                    CharacterState targetState = args.Context.Characters.
                        MemberDataContainer[occupier].PlayerState;

                    Vector3Int retreatDestination = SkillUtility.GetClassKnockbackPattern(
                        userState.Character.ClassType, userState.Position, targetState.Position);

                    TileId defender = args.Context.World.GetOccupyingTileAt(retreatDestination);

                    // Check for an enemy defender at the would-be knockback location, 
                    // if there is a defender at that position, this skill fails.
                    if (defender != null && defender.FactionId == occupier.FactionId) 
                    {
                        result.OnFail($"Enemy is reinforced at {dest} by forces at {retreatDestination}.", args.Context);
                    }
                    // If there is no defender, the skill succeeds.
                    else 
                    {
                        gameContext = SkillUtility.Combat(
                            args.Context,
                            args.UserId,
                            occupier);
                        result.OnSuccess($"Attacking enemy at {dest}", gameContext);
                    }
                }
                // Cannot attack ally occupier, reject this activation.
                else 
                {
                    gameContext = args.Context;
                    result.OnFail("Cannot attack allied character.", gameContext);
                }

                return result;
            }
        }
    }
}


