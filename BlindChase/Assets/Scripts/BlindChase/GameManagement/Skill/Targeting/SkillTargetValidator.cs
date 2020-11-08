using System.Collections.Generic;
using UnityEngine;
using BlindChase.Utility;

namespace BlindChase.GameManagement
{
    public static partial class TargetEvaluation
    {
        // Get all valid targets for a command.
        public static List<Vector3Int> InspectSkillTargetOptions(int skillId, ObjectId userId, Vector3Int userCoord, RangeOffsetMap range, WorldContext world)
        {
            List<Vector3Int> allTargets = range.ApplyRangeOffsets(userCoord);
            List<Vector3Int> toRemove = new List<Vector3Int>();
            // Check and remove any invalid targets.
            foreach (Vector3Int targetPos in allTargets)
            {
                ObjectId target = world.GetOccupyingTileAt(targetPos);
                bool isValid = world.IsPositionInBound(targetPos) && IsSkillTargetSelectionValid(target, userId, skillId);
                if (!isValid)
                {
                    toRemove.Add(targetPos);
                }
            }

            foreach (Vector3Int removeThis in toRemove)
            {
                allTargets.Remove(removeThis);
            }
            return allTargets;
        }

        public static bool IsSkillTargetSelectionValid(
            in ObjectId selectedId, 
            in ObjectId currentActiveId, 
            in int skillId)
        {
            AllowedTarget currentTargetType = GetSkillTargetingType(selectedId, currentActiveId);
            AllowedTarget validTargets = SkillManager.GetSkillData(skillId).AllowedTargets;
            // Check if target has nothing in common with the skill's target requirement
            bool isValid = (currentTargetType & validTargets) != AllowedTarget.None;
            return isValid;
        }

        static AllowedTarget GetSkillTargetingType(in ObjectId selectedId, in ObjectId currentActiveId)
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
            else if (selectedId == null)
            {
                selectionType = AllowedTarget.Position;
            }

            return selectionType;
        }
    }

}


