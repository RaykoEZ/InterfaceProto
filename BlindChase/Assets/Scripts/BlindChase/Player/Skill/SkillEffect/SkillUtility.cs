using UnityEngine;

namespace BlindChase
{
    public static class SkillUtility 
    {
        public static GameContextCollection DealDamage(SkillEffectArgs args)
        {
            Debug.Log("Damage");

            return args.Context;
        }

        public static Vector3Int Knockback(Vector3Int targetPosition, Vector3Int direction, int knockbackScale) 
        {
            return targetPosition + knockbackScale * direction;
        }

        public static GameContextCollection RestoreHP(SkillEffectArgs args)
        {
            Debug.Log("Heal");

            return args.Context;
        }

    } 
}


