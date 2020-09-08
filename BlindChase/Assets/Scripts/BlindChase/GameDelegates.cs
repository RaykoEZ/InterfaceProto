using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;

namespace BlindChase
{
    public delegate void OnWorldStateUpdate(WorldStateContext context);
    public delegate void OnCharacterUpdate(CharacterContext newContext);
    public delegate void OnCharacterDefeated(TileId id);

    public delegate void OnStateChange();
    public delegate void OnGameStateTransition(Type gameStateType);
    public delegate void OnCharacterTileActivate(TileId activeCharacterId);

    public delegate void OnPlayerCommand<T>(T info)
        where T : CommandEventInfo;
    public delegate void OnTileTriggered();

    public delegate void OnSkillTargetConfirmed(HashSet<Vector3> targets);
    public delegate void OnSkillClicked(int skillId, int skillLevel);
    public delegate void OnSkillCancelled();
    public delegate void OnSkillConfirmed();
}


