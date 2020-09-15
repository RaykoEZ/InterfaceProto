using UnityEngine;
namespace BlindChase
{
    public partial class SkillEffect
    {
        // This contains all possible calls available for a skill effect.
        static partial class SkillOperation
        {
            public static GameContextCollection Strike(SkillEffectArgs args) 
            {
                if (args.TargetId == null) 
                {
                    return args.Context;
                }

                CharacterState target = args.Context.Characters.MemberDataContainer[args.TargetId].PlayerState;
                Debug.Log(target.CurrentHP);

                int baseValue = args.SkillData.BaseValue;
                target = SkillUtility.DealDamage(baseValue, ref target);

                Debug.Log(target.CurrentHP);
                return args.Context;
            }

            public static GameContextCollection FirstAid(SkillEffectArgs args) 
            {
                GameContextCollection gameContext = SkillUtility.RestoreHP(args);

                return gameContext;
            }

            public static GameContextCollection BasicMovement(SkillEffectArgs args) 
            {
                Vector3Int dest = args.TargetCoord;
                CharacterState state = args.Context.Characters.
                    MemberDataContainer[args.UserId].PlayerState;
                
                TileId occupier = args.TargetId;
                GameContextCollection gameContext;
                // If no one is occupying the destination, move as usual
                // Else, enter combat
                if (occupier == null) 
                {
                    gameContext = SkillUtility.MoveTo(
                        args.Context,
                        args.UserId,
                        dest,
                        state.Position);
                }
                else 
                {
                    gameContext = SkillUtility.Combat(
                        args.Context,
                        args.UserId,
                        occupier);
                }

                return gameContext;
            }
        }
    }
}


