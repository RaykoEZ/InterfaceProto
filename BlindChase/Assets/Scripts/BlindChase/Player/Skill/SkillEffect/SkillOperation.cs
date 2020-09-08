namespace BlindChase
{
    public partial class SkillEffect
    {
        // This contains all possible calls available for a skill effect.
        static partial class SkillOperation
        {
            public static GameContextCollection Strike(SkillEffectArgs args) 
            {
                GameContextCollection gameContext = args.Context;

                SkillUtility.DealDamage(args);

                return gameContext;
            }


            public static GameContextCollection FirstAid(SkillEffectArgs args) 
            {
                GameContextCollection gameContext = args.Context;

                SkillUtility.RestoreHP(args);

                return gameContext;
            }
        }
    }
}


