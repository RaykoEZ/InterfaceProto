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

        public static bool IsSelectionValid(int skillId, AllowedTarget target)
        {
            AllowedTarget validTargets = SkillManager.GetSkillData(skillId).AllowedTargets;
            // Check if target has nothing in common with the skill's target requirement
            bool isValid = (target & validTargets) != AllowedTarget.None;
            return isValid;
        }
    }

}


