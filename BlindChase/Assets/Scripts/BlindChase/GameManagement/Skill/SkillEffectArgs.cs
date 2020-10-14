using UnityEngine;

namespace BlindChase.GameManagement
{
    public class SkillEffectArgs 
    {
        public GameContextRecord Context { get; private set; }
        public Vector3Int TargetCoord { get; private set; }

        public ObjectId UserId { get; private set; }

        public ObjectId TargetId 
        { 
            get 
            {
                return Context.WorldRecord.GetOccupyingTileAt(TargetCoord); 
            } 
        }


        public CharacterState UserState { get { return Context.CharacterRecord.MemberDataContainer[UserId].PlayerState; } }
        public CharacterState TargetState { get { return Context.CharacterRecord.MemberDataContainer[TargetId].PlayerState; } }


        public SkillParameters SkillData { get; private set; }
        
        public SkillEffectArgs(GameContextRecord context, Vector3Int target, ObjectId user, SkillParameters args) 
        {
            Context = context;
            TargetCoord = target;
            UserId = user;
            SkillData = args;
        }
    }
}


