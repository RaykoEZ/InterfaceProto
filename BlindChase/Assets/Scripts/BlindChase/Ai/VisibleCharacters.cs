using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Utility;

namespace BlindChase.Ai
{
    // Contains all character states detected by Npc
    public class VisibleCharacters
    {
        public List<CharacterState> VisibleAllies { get; private set; }
        public List<CharacterState> VisibleEnemies { get; private set; }

        public VisibleCharacters(List<CharacterState> allies, List<CharacterState> enemies) 
        {
            VisibleAllies = allies;
            VisibleEnemies = enemies;
        }

        public static VisibleCharacters GetVisibleCharacters(in GameContextRecord contextRecord, CharacterState npc, RangeMap range)
        {
            CharacterContext c = contextRecord.CharacterRecord;
            WorldContext w = contextRecord.WorldRecord;

            // Get all visible characters in range.
            List<CharacterState> alliesInRange = new List<CharacterState>();
            List<CharacterState> enemiesInRange = new List<CharacterState>();
            Vector3Int origin = npc.Position;
            // Search for characters in the range map.
            foreach (Vector3Int offset in range.OffsetsFromOrigin) 
            {
                ObjectId occupierId = w.GetOccupyingTileAt(origin + offset);
                bool isInRange = occupierId != null;
                bool isAlliy = occupierId?.FactionId == npc.ObjectId.FactionId;
                if (isInRange && isAlliy)
                {
                    CharacterState character = c.MemberDataContainer[occupierId].PlayerState;
                    alliesInRange.Add(character);
                }
                else if (isInRange && !isAlliy)
                {
                    CharacterState character = c.MemberDataContainer[occupierId].PlayerState;
                    enemiesInRange.Add(character);
                }
            }

            VisibleCharacters result = new VisibleCharacters(alliesInRange, enemiesInRange);
            return result;
        }

    }



}


