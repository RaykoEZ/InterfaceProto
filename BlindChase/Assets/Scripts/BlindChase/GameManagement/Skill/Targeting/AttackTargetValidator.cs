using System.Collections.Generic;
using UnityEngine;
using BlindChase.Utility;

namespace BlindChase.GameManagement
{
    // Validator functions for Attacking
    public static partial class TargetEvaluation
    {
        public static HashSet<AdvancementOption> InspectMovementOption(ObjectId userId, Vector3Int userCoord, RangeOffsetMap moveRange, GameContextRecord context)
        {
            List<Vector3Int> targets = moveRange.ApplyRangeOffsets(userCoord);

            HashSet<AdvancementOption> ret = new HashSet<AdvancementOption>();

            WorldContext world = context.WorldRecord;
            foreach (Vector3Int targetPos in targets)
            {
                bool isValid = IsAttackTargetValid(
                    userId,
                    targetPos,
                    context, 
                    out bool isFreeSpace, out bool defenderVulnerable, out bool defenderDefeatable);

                if (isValid)
                {
                    ObjectId targetId = world.GetOccupyingTileAt(targetPos);
                    AdvancementOption option = new AdvancementOption(targetId, targetPos, isFreeSpace, defenderVulnerable, defenderDefeatable);
                    ret.Add(option);
                }
            }

            return ret;
        }


        // Overload to return whether the target is occupied AND attackable/defeated
        public static bool IsAttackTargetValid(
            in ObjectId attacker,
            in Vector3Int targetCoord,
            GameContextRecord context,
            out bool isOpenSpace,
            out bool isOccupierVulnerable,
            out bool isOccupierDefeatable)
        {
            CharacterContext c = context.CharacterRecord;
            WorldContext w = context.WorldRecord;

            // Check if target is inside the map
            bool isInBound = w.IsPositionInBound(targetCoord);
            if (!isInBound)
            {
                isOccupierVulnerable = false;
                isOccupierDefeatable = false;
                isOpenSpace = false;
                return false;
            }

            // No occupier, free to move onto
            ObjectId occupier = w.GetOccupyingTileAt(targetCoord);
            isOpenSpace = occupier == null;
            // Check if occupier is enemy. You cannot attack your allies. 
            // True by default due to empty locations being traversable.
            bool occupierIsEnemy = true;
            Vector3Int retreatDestination;
            CharacterState attackerState = c.MemberDataContainer[attacker].PlayerState;
            bool isRetreatImpossible = false;
            bool isTakingLethalDamage = false;
            ObjectId defender = null;
            if (!isOpenSpace)
            {
                CharacterState targetState = c.MemberDataContainer[occupier].PlayerState;

                retreatDestination = ActionSimulation.GetClassKnockbackPattern(
                            attackerState.Character.ClassType, attackerState.Position, targetState.Position);
              
                float threatValue = targetState.EstimateThreat(attackerState);

                isTakingLethalDamage = threatValue >= targetState.CurrentHP;

                defender = w.GetOccupyingTileAt(retreatDestination);
                // If retreat position is blocked by an enemy OR a boundary OR taking lethal damage, retreat is not possible.
                isRetreatImpossible = IsRetreatDenied(occupier, defender, retreatDestination, w);
                occupierIsEnemy = occupier.FactionId != attacker.FactionId;
            }

            Vector3Int origin = c.MemberDataContainer[attacker].PlayerState.Position;
            Vector3Int offset = targetCoord - origin;
            // You cannot stay in the same position.
            bool isNotStationary = offset != Vector3Int.zero;

            // No defender for the occupier, valid target to attack
            isOccupierVulnerable = occupierIsEnemy && defender == null;

            // If there is a character not allied with the occupier in the way of occupier's retreat, this is a valid target.
            isOccupierDefeatable = (occupierIsEnemy && isRetreatImpossible) || isTakingLethalDamage;

            // Sum all allowed checks
            bool isDestinationPossible = isOpenSpace || isOccupierVulnerable || isOccupierDefeatable;

            return isNotStationary && isDestinationPossible;
        }



        public static bool IsRetreatDenied(ObjectId retreatingId, ObjectId defender, Vector3Int retreatingPos, WorldContext world) 
        {
            bool retreatImpossible = (defender != null && defender.FactionId != retreatingId.FactionId) || !world.IsPositionInBound(retreatingPos);
            return retreatImpossible;
        }

        public static bool IsAttackTargetValid(
            in ObjectId attacker,
            in Vector3Int targetCoord,
            GameContextRecord context)
        {
            bool isValid = IsAttackTargetValid(attacker, targetCoord, context, out bool isOpenSpace, out bool isVulnerable, out bool isDefeatable);
            return isValid;
        }

    }

}


