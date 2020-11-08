using UnityEngine;

namespace BlindChase.GameManagement
{
    // Stores info about a valid target position on the board.
    public struct AdvancementOption
    {
        public ObjectId TargetId;
        public Vector3Int Position;
        public bool IsOpenSpace;
        public bool IsDefenderVulnerable;
        public bool IsDefenderDefeatable;

        public AdvancementOption(ObjectId id, Vector3Int pos, bool isFreeSpace, bool vulnerable, bool defeatable) 
        {
            TargetId = id;
            Position = pos;
            IsOpenSpace = isFreeSpace;
            IsDefenderVulnerable = vulnerable;
            IsDefenderDefeatable = defeatable;
        }
    }

}


