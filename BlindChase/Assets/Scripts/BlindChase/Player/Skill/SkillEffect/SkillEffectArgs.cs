using UnityEngine;

namespace BlindChase
{
    public class SkillEffectArgs 
    {
        public GameContextCollection Context { get; private set; }
        public Vector3Int TargetCoord { get; private set; }

        public TileId TargetId 
        { 
            get 
            {
                return Context.World.GetOccupyingTileAt(TargetCoord); 
            } 
        }

        public TileId UserId { get; private set; }
        public SkillDataItem SkillData { get; private set; }
        
        public SkillEffectArgs(GameContextCollection context, Vector3Int target, TileId user, SkillDataItem args) 
        {
            Context = context;
            TargetCoord = target;
            UserId = user;
            SkillData = args;
        }
    }
}


