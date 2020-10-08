using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;

namespace BlindChase.Events
{
    public delegate void OnWorldStateUpdate(WorldContext context);
    public delegate void OnCharacterUpdate(CharacterContext newContext);

    public delegate void OnSkillActivate(EventInfo id);
    public delegate void OnCharacterAttack(EventInfo id);
    public delegate void OnTakeDamage(EventInfo id);

    public delegate void OnCharacterDefeated(EventInfo id);
    public delegate void OnLeaderDefeated(EventInfo id);

    public delegate void OnTileSelect(EventInfo tileCoord);

    public delegate void OnStateChange();

    public delegate void OnTurnStateTransition(Type gameStateType);
    public delegate void OnCharacterActivate(ObjectId activeCharacterId);

    public delegate void OnPlayerCommand<T>(T info)
        where T : EventInfo;
    public delegate void OnTileTriggered();

    public delegate void OnSkillTargetConfirmed(HashSet<Vector3> targets);
    public delegate void OnSkillClicked(int skillId, int skillLevel);
    public delegate void OnSkillCancelled();
    public delegate void OnSkillConfirmed();
}


