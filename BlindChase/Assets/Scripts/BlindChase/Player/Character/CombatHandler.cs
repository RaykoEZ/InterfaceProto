using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{
    public class CombatHandler
    {
        WorldStateContext m_worldContext;
        CharacterContext m_characterContext;
        CharacterTileManager m_tileManager;
        public event OnCharacterDefeated OnCharacterDefeat = default;

        public void SetupContext(WorldStateContext w, CharacterContext c, CharacterTileManager tileManager) 
        {
            m_worldContext = w;
            m_characterContext = c;
            m_tileManager = tileManager;
        }

        public GameContextCollection MoveTarget(TileId movingTargetId, TileId currentActiveCharacterId, Vector3 destination, Vector3 origin)
        {
            Vector3Int dest = m_worldContext.WorldMap.LocalToCell(destination);
            CharacterState state = m_characterContext.MemberDataContainer[movingTargetId].PlayerState;

            CharacterContext context = new CharacterContext(m_characterContext);
            WorldStateContext world = new WorldStateContext(m_worldContext);
            // Update Board State
            TileId occupier = m_worldContext.GetOccupyingTileAt(dest);

            // If we find a character in the destination, perform some combat logic
            if (occupier != null && movingTargetId == currentActiveCharacterId)
            {
                // Get the result character state after combat.
                Debug.Log("Attacking occupying player.");
                context.MemberDataContainer[occupier] = OnCombat(
                    state,
                    m_characterContext.MemberDataContainer[occupier].PlayerState,
                    m_worldContext);
            }
            else
            {
                Vector3Int o = m_worldContext.WorldMap.LocalToCell(origin);
                world.UpdateBoardState(dest, movingTargetId);
                world.RemoveBoardPiece(o);
            }

            Vector3 offset = destination - origin;
            m_tileManager.MoveTile(movingTargetId, offset);
            state.Position = dest;

            CharacterStateContainer container = new CharacterStateContainer(
                m_characterContext.MemberDataContainer[movingTargetId].PlayerTransform, state);

            context.MemberDataContainer[movingTargetId] = container;

            GameContextCollection gameContext = new GameContextCollection { World = world, Characters = context };

            return gameContext;
        }

        CharacterStateContainer OnCombat(CharacterState attacker, CharacterState target, WorldStateContext world)
        {
            Vector3Int knockbackPos = GetClassKnockbackPattern(attacker.Character.ClassType, attacker.Position, target.Position);
            TileId occupier = world.GetOccupyingTileAt(knockbackPos);

            // Character defeated if not able to be knocked back.
            if (occupier != null && occupier != attacker.TileId)
            {
                OnTargetDefeat(attacker, target, world);
            }
            else
            {
                OnTargetRetreat(attacker, target, knockbackPos, world);
            }

            CharacterStateContainer container = new CharacterStateContainer(
                m_characterContext.MemberDataContainer[target.TileId].PlayerTransform, target);

            return container;
        }

        Vector3Int GetClassKnockbackPattern(CharacterClassType attackerClass, Vector3Int attackerPos, Vector3Int targetPos) 
        {
            Vector3 dist = targetPos - attackerPos;
            Vector3Int direction = Vector3Int.RoundToInt(dist / dist.magnitude);
            Vector3Int knockbackPos;

            switch (attackerClass)
            {
                case CharacterClassType.Swordsman:
                    knockbackPos = attackerPos;
                    break;
                case CharacterClassType.Assassin:
                    knockbackPos = SkillUtility.Knockback(targetPos, direction, 2);
                    break;
                default:
                    knockbackPos = SkillUtility.Knockback(targetPos, direction, 1);
                    break;
            }

            return knockbackPos;
        }

        void OnTargetRetreat(CharacterState attacking, CharacterState retreating, Vector3Int retreatingDest, WorldStateContext world) 
        {
            Vector3 dest = world.WorldMap.CellToWorld(retreatingDest);
            Vector3 origin = world.WorldMap.CellToWorld(retreating.Position);
            Vector3 offset = dest - origin;

            world.UpdateBoardState(retreating.Position, attacking.TileId);
            world.RemoveBoardPiece(attacking.Position);
            world.UpdateBoardState(retreatingDest, retreating.TileId);

            m_tileManager.MoveTile(retreating.TileId, offset);
            retreating.Position = retreatingDest;
        }

        void OnTargetDefeat(CharacterState attacker, CharacterState target, WorldStateContext world)
        {
            target.IsActive = false;
            Debug.Log("Piece Defeated");
            world.RemoveBoardPiece(target.Position);
            world.RemoveBoardPiece(attacker.Position);
            world.UpdateBoardState(target.Position, attacker.TileId);
            m_tileManager.HideTile(target.TileId);
            OnCharacterDefeat?.Invoke(target.TileId);
        }
    }
}


