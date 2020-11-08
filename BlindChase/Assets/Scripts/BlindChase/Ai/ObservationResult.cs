using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    // Contains all character states detected by Npc
    public class ObservationResult
    {       
        HashSet<AdvancementOption> m_enemyAttackRange;
        public List<ObjectId> AllyIds { get; private set; }
        public List<ObjectId> EnemyIds { get; private set; }
        public HashSet<AdvancementOption> EnemyAttackRange { get { return new HashSet<AdvancementOption>(m_enemyAttackRange); } }

        protected ObservationResult(List<ObjectId> allies, List<ObjectId> enemies, HashSet<AdvancementOption> enemyAttackRange) 
        {
            AllyIds = allies;
            EnemyIds = enemies;
            m_enemyAttackRange = enemyAttackRange;
        }

        public static ObservationResult CharactersInRange(
            in GameContextRecord contextRecord,
            RangeMapDatabase rangeDb,
            CharacterState npc,
            RangeOffsetMap range)
        {
            CharacterContext c = contextRecord.CharacterRecord;
            WorldContext w = contextRecord.WorldRecord;

            // Get all visible characters in range.
            List<ObjectId> alliesInRange = new List<ObjectId>();
            List<ObjectId> enemiesInRange = new List<ObjectId>();
            HashSet<AdvancementOption> enemyAttackRange = new HashSet<AdvancementOption>();
            Vector3Int origin = npc.Position;
            // Search for characters in the range map.
            foreach (Vector3Int offset in range.OffsetsFromOrigin) 
            {
                ObjectId occupierId = w.GetOccupyingTileAt(origin + offset);
                bool exists = occupierId != null;
                bool isAlliy = occupierId?.FactionId == npc.ObjectId.FactionId;
                if (exists && isAlliy)
                {
                    alliesInRange.Add(occupierId);
                }
                else if (exists && !isAlliy)
                {
                    enemiesInRange.Add(occupierId);

                    // Add positions threatened by enemies.
                    CharacterState occupierState = c.MemberDataContainer[occupierId].PlayerState;
                    CharacterClassType enemyClass = occupierState.Character.ClassType;
                    RangeOffsetMap attackRangeOffsets = rangeDb.GetAttackRangeMap(enemyClass);
                    HashSet<AdvancementOption> attackRange = 
                        TargetEvaluation.InspectMovementOption(occupierId, occupierState.Position, attackRangeOffsets, contextRecord);

                    enemyAttackRange.UnionWith(attackRange);
                }
            }

            ObservationResult result = new ObservationResult(alliesInRange, enemiesInRange, enemyAttackRange);
            return result;
        }

    }



}


