using System;
using UnityEngine;

namespace BlindChase 
{
    public static class TileDisplayKeywords
    {
        public const string MOVEMENT_RANGE = "MOVE";
        public const string SKILL_RANGE = "SKILL";
        public const string PLAYER = "P";
        public const string ENVIRONMENT = "E";
        public const string INTERACTABLE = "I";
    }

    public class TileId : IEquatable<TileId>
    {
        public CommandTypes TypeId { get; private set; }
        public string FactionId { get; private set; }
        public string UnitId { get; private set; }
        public TileId(CommandTypes typeId, string factionId, string unitId) 
        {
            TypeId = typeId;
            FactionId = factionId;
            UnitId = unitId;
        }
        public TileId(TileId tileId)
        {
            TypeId = tileId.TypeId;
            FactionId = tileId.FactionId;
            UnitId = tileId.UnitId;
        }
        public static bool operator ==(TileId t1, TileId t2) 
        {
            if(t2 is null) 
            {
                return false;
            }

            return t1.TypeId == t2.TypeId &&
                t1.FactionId == t2.FactionId &&
                t1.UnitId == t2.UnitId;
        }

        public static bool operator != (TileId t1, TileId t2)
        {
            return t1.TypeId != t2.TypeId ||
                t1.FactionId != t2.FactionId ||
                t1.UnitId != t2.UnitId;
        }

        public bool Equals(TileId obj)
        {
            return this == obj;
        }

        public override bool Equals(object obj)
        {
            if (obj is TileId Id) 
            {
                return Equals(Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 
                $"{TypeId}/{FactionId}/{UnitId}".GetHashCode();
        }

    }

}


