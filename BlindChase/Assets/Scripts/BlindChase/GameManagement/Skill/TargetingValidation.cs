using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    public static class TargetingValidation
    {
        public static AllowedTarget GetSelectionType(ObjectId selectedId, ObjectId currentActiveId) 
        {
            AllowedTarget selectionType = AllowedTarget.None;

            if (selectedId != null && selectedId.FactionId == currentActiveId.FactionId)
            {
                selectionType = AllowedTarget.Ally;
            }
            else if (selectedId != null && selectedId.FactionId != currentActiveId.FactionId)
            {
                selectionType = AllowedTarget.Enemy;
            }
            // If it is an unoccupied position, it will have no object (ObjectId) living in there.
            else if( selectedId == null) 
            {
                selectionType = AllowedTarget.Position;
            }

            return selectionType;
        }

        public static bool IsDestinationValid(ObjectId attacker, Vector3Int targetCoord, GameContextRecord context) 
        {
            CharacterContext c = context.CharacterRecord;
            WorldContext w = context.WorldRecord;

            ObjectId occupier = w.GetOccupyingTileAt(targetCoord);
            // No occupier, free to move onto
            CharacterState userState = c.MemberDataContainer[attacker].PlayerState;
            bool isOpenSpace = occupier == null;
            // Check if occupier is enemy. You cannot attack your allies. 
            // True by default due to empty locations being traversable.
            bool occupierIsEnemy = true; 
            ObjectId defender = null;
            if (!isOpenSpace) 
            {
                CharacterState targetState = c.MemberDataContainer[occupier].PlayerState;

                Vector3Int retreatDestination = ActionSimulation.GetClassKnockbackPattern(
                            userState.Character.ClassType, userState.Position, targetState.Position);

                defender = w.GetOccupyingTileAt(retreatDestination);
                occupierIsEnemy = occupier.FactionId != attacker.FactionId;
            }

            Vector3Int origin = c.MemberDataContainer[attacker].PlayerState.Position;
            Vector3Int offset = targetCoord - origin;
            // You cannot stay in the same position.
            bool isNotStationary = offset != Vector3Int.zero;
            // No defender for the occupier, valid target to attack
            bool isOccupierVulnerable = occupierIsEnemy && defender == null;
            // If there is a character not allied with the occupier in the way of occupier's retreat, this is a valid target.
            bool isOccupierDeafeatable = occupierIsEnemy && defender != null && defender.FactionId != occupier.FactionId;
            // Sum all allowed checks
            bool isDestinationPossible = isOpenSpace || isOccupierVulnerable || isOccupierDeafeatable;

            return isNotStationary && isDestinationPossible;
        }

        public static bool IsSkillTargetSelectionValid(int skillId, AllowedTarget target)
        {
            AllowedTarget validTargets = SkillManager.GetSkillData(skillId).AllowedTargets;
            // Check if target has nothing in common with the skill's target requirement
            bool isValid = (target & validTargets) != AllowedTarget.None;
            return isValid;
        }
    }

}


