using UnityEngine;

namespace BlindChase.GameManagement
{
    public class SkillEffectArgs 
    {
        public GameContextCollection Context { get; private set; }
        public Vector3Int TargetCoord { get; private set; }

        public ObjectId UserId { get; private set; }

        public ObjectId TargetId 
        { 
            get 
            {
                return Context.World.GetOccupyingTileAt(TargetCoord); 
            } 
        }


        public CharacterState UserState { get { return Context.Characters?.MemberDataContainer[UserId].PlayerState; } }
        public CharacterState TargetState { get { return Context.Characters?.MemberDataContainer[TargetId].PlayerState; } }


        public SkillDataItem SkillData { get; private set; }
        
        public SkillEffectArgs(GameContextCollection context, Vector3Int target, ObjectId user, SkillDataItem args) 
        {
            Context = context;
            TargetCoord = target;
            UserId = user;
            SkillData = args;
        }
    }
}


