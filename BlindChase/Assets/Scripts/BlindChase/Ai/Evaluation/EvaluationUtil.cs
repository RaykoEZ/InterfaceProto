using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    public static class EvaluationUtil
    {
        // Returns the Hp lost in proportion to maxHp.
        public static float HpLossFactor(int maxHp, int currentHp)
        {
            float diff = maxHp - currentHp;
            float ret = maxHp <= 0 ? 1.0f : diff / maxHp;
            return ret;
        }

        // Tends to 1 when outnumbered.
        // When evenly matched or no enemies, factor == 1/2.
        public static float EnemyPresenceMod(int numEnemies, int numAllies)
        {
            return numEnemies <= 0 ? 0.5f : numEnemies / (numAllies + numEnemies);
        }

        // Checking vital status for current character.
        public static float SurvivalRating(ObjectId userId, CharacterContext contextBefore, CharacterContext contextAfter) 
        {
            float allyPowerAfter = contextAfter.FactionPowerValue[userId.FactionId];
            float allyPoerBefore = contextBefore.FactionPowerValue[userId.FactionId];
            // We use the context before simulation as demominator to factor in any defeated allies. 
            float ret = allyPowerAfter - allyPoerBefore;
            return ret;
        }

        // Check damage dealt/character defeated etc and score the performance.
        public static float AttackRating(ObjectId userId, SimulationResult combatResult,  CharacterContext contextBefore) 
        {
            float baseScore = 0f;
            CharacterContext characterContextAfter = combatResult.ResultContext.CharacterRecord;

            // Check all enemy vital changes after simulation
            foreach (KeyValuePair<string, float> factionPowerBefore in contextBefore.FactionPowerValue) 
            {
                string factionId = factionPowerBefore.Key;            
                if (factionId != userId.FactionId)
                {
                    float powerDiff = Math.Max(0f, factionPowerBefore.Value - characterContextAfter.FactionPowerValue[factionId]);
                    baseScore += powerDiff;
                }
            }

            // Check for defeated characters to award points for each defeated enemy.
            foreach (CharacterState defeated in combatResult.DefeatedCharacters) 
            {
                bool isEnemy = defeated.ObjectId.FactionId != userId.FactionId;
                float score = defeated.Character.ClassType == CharacterClassType.Master ? 9999f : defeated.Vitality;
                baseScore += isEnemy ? score : -score;
            }

            int enemyFactionCount = Mathf.Max(1, contextBefore.FactionPowerValue.Count - 1);

            baseScore /= enemyFactionCount;

            return baseScore;
        }

        // Check how flexible/vulnerable/advantageous the current character position is
        public static void EvaluatePositioning(
            CharacterState userState,
            RangeOffsetMap userMoveRange,
            ObservationResult afterSim, 
            GameContextRecord contextAfter,
            out float riskRating,
            out float advantageScaler) 
        {
            // Check for immediate threats.
            riskRating = 1.0f;
            // TEMP FACTOR
            float selfValue = userState.Character.ClassType == CharacterClassType.Master ? 1.5f : 1.0f;
            foreach (ObjectId enemyId in afterSim.EnemyIds) 
            {
                TargetEvaluation.IsAttackTargetValid(enemyId, userState.Position, contextAfter,
                   out bool isFreeSpace, out bool canBeAttacked, out bool canBeDefeated);

                if (canBeAttacked) 
                {
                    riskRating += 0.25f;
                }

                if (canBeDefeated) 
                {
                    riskRating +=  0.5f;
                }

                riskRating *= selfValue;
            }

            // Check for future threats & variety of moves.
            HashSet<AdvancementOption> futureMoves = TargetEvaluation.InspectMovementOption(
                userState.ObjectId,
                userState.Position,
                userMoveRange,
                contextAfter);

            HashSet<AdvancementOption> unSafeMoves = afterSim.EnemyAttackRange;
            futureMoves.ExceptWith(unSafeMoves);           
            // Check for number of valid attack positions on the next move, and number of vulnerable/defeatable enemies. 
            advantageScaler = PositionAdvantage(contextAfter, futureMoves);
        }

        static float PositionAdvantage(
            GameContextRecord contextAfter,
            HashSet<AdvancementOption> moveOptions)       
        {
            float ret = 1.0f;
            CharacterContext c = contextAfter.CharacterRecord;
            foreach(AdvancementOption option in moveOptions)
            {
                if(!option.IsOpenSpace) 
                {
                    CharacterState target = c.MemberDataContainer[option.TargetId].PlayerState;
                    CharacterClassType targetClass = target.Character.ClassType;
                    float bonusMod = targetClass == CharacterClassType.Master ? 1.5f : 1.0f;

                    ret += option.IsDefenderVulnerable ? 0.25f : 0f;
                    ret += option.IsDefenderDefeatable ? 0.5f : 0f;
                    ret *= bonusMod;
                }
            }
     
            return ret;
        }

    }

}


