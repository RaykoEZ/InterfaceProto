using System;

namespace BlindChase 
{
    public class ObjectId : IEquatable<ObjectId>
    {
        public string FactionId { get; private set; }
        public string UnitId { get; private set; }
        public CommandTypes CommandType { get; private set; }
        public string NPCId { get; private set; }

        public ObjectId(
            string factionId, 
            string unitId,
            CommandTypes typeId = CommandTypes.NONE,
            string npcId = "") 
        {
            FactionId = factionId;
            UnitId = unitId;
            CommandType = typeId;
            NPCId = npcId;
        }
        public ObjectId(ObjectId tileId)
        {
            FactionId = tileId.FactionId;
            UnitId = tileId.UnitId;
            CommandType = tileId.CommandType;
            NPCId = tileId.NPCId;
        }
        public static bool operator ==(ObjectId t1, ObjectId t2) 
        {
            return 
                t1?.FactionId == t2?.FactionId &&
                t1?.UnitId == t2?.UnitId &&
                t1?.CommandType == t2?.CommandType &&
                t1?.NPCId == t2?.NPCId;
        }

        public static bool operator != (ObjectId t1, ObjectId t2)
        {
            return 
                t1?.FactionId != t2?.FactionId ||
                t1?.UnitId != t2?.UnitId ||
                t1?.CommandType != t2?.CommandType ||
                t1?.NPCId != t2?.NPCId;     
        }

        public static bool CompareFactionAndUnitId(ObjectId t1, ObjectId t2) 
        { 
            return t1?.FactionId == t2?.FactionId &&
                t1?.UnitId == t2?.UnitId;
        }

        public bool Equals(ObjectId obj)
        {
            return this == obj;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectId Id) 
            {
                return Equals(Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 
                $"{FactionId}/{UnitId}/{CommandType}/{NPCId}".GetHashCode();
        }

    }

}


