using System;

namespace BlindChase.Ai
{
    public static class EvaluationUtil 
    {
        // Methods used to assess game state values and derrive decision weights/scores for the AI planner.

        //NOTE: All "numAllies" parameter will need to include self, always > 0.
        // As HP lowers & A.i is outnumbered by enemies, the urge to survive should increase.
        public static float SelfSurvivalPriority(float defaultMod, int maxHp, int currentHp, int numEnemies, int numAllies)
        {
            float enemyPresence = EnemyPresenceMod(numEnemies, numAllies);
            // numAllies will always include self.

            float lossFactor = enemyPresence * HpLossFactor(maxHp, currentHp);
           
            float result = defaultMod * lossFactor;
            return result;
        }

        // HpLoss : fraction of Hp loss for the protectee
        public static float AgressionPriority(float defaultMod, float offenseFactor, int maxHp, int currentHp)
        {
            float hpFactor = 1 - HpLossFactor(maxHp, currentHp);

            // If we can't find any defeatable enemies, still assign 0.5f multiplier to this priority as opposed to 0.
            // Task planner will handle offensive setups if this task wins out.

            // 0.5f = EnemyPresenceMod(0, 1), which means:
            // - NPC has no other allies in sight.
            // - There is no enemy in sight.
            float result = hpFactor * offenseFactor* defaultMod;
            return result;
        }

        // Modify priority score for protecting an object/location.
        // Distance : sum of component-wise diff of positions.
        // HpLoss : fraction of Hp loss for the protectee
        // numEnemies/numAllies : number of actve characters on opposing/own faction.
        public static float ProtectionPriority(
            float defaultMod,
            float minEnemyDistanceToVip,
            float minAllyDistanceToVip,
            int numEnemyNearVip,
            int numAlliesNearVip)
        {
            // when enemy is closer to VIP, the scaling for the modifier will increase drastically.
            float distanceRatio = minAllyDistanceToVip / minEnemyDistanceToVip;
            float result = distanceRatio * EnemyPresenceMod(numEnemyNearVip, numAlliesNearVip) * defaultMod;

            return result;
        }

        // Returns the Hp lost in proportion to maxHp.
        static float HpLossFactor(int maxHp, int currentHp)
        {
            return maxHp <= 0? 1.0f : (maxHp - currentHp) / maxHp;
        }

        // Tends to 1 when outnumbered.
        // When evenly matched or no enemies, factor == 1/2.
        static float EnemyPresenceMod(int numEnemies, int numAllies)
        {
            return numEnemies <= 0 ? 0.5f :  numEnemies / (numAllies + numEnemies);
        }

        // Determine whether a target is weak enough to defeat by judging its hp loss
        public static bool IsTargetWeak(int targetHp, int targetMaxHp, float tolerance)
        {
            return (targetHp / targetMaxHp) < tolerance;
        }
    }

}


