﻿using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.GameManagement
{
    // Used to obtain results for character actions without directly modifying current game state.
    public static class ActionSimulation
    {
        public static GameContextRecord DealDamage(GameContextRecord context, ObjectId targetd, int baseValue)
        {
            CharacterContext characters = context.CharacterRecord;
            WorldContext world = context.WorldRecord;

            CharacterState state = characters.MemberDataContainer[targetd].PlayerState;
            int netDamage = Mathf.Max(baseValue - state.CurrentDefense, 0);
            state.CurrentHP -= netDamage;

            GameContextRecord newContext = new GameContextRecord(world, characters);
            return newContext;
        }

        public static GameContextRecord RestoreHP(GameContextRecord context, ObjectId targetd, int baseValue)
        {
            CharacterContext characters = context.CharacterRecord;
            WorldContext world = context.WorldRecord;
            CharacterState state = characters.MemberDataContainer[targetd].PlayerState;

            state.CurrentHP = Mathf.Min(state.CurrentHP + baseValue, state.Character.MaxHP);
            GameContextRecord newContext = new GameContextRecord(world, characters);

            return newContext;
        }

        public static GameContextRecord RestoreSP(GameContextRecord context, ObjectId targetd, int baseValue)
        {
            CharacterContext characters = context.CharacterRecord;
            WorldContext world = context.WorldRecord;
            CharacterState state = characters.MemberDataContainer[targetd].PlayerState;

            state.CurrentSP = Mathf.Min(state.CurrentSP + baseValue, state.Character.MaxSP);
            GameContextRecord newContext = new GameContextRecord(world, characters);

            return newContext;
        }

        public static GameContextRecord SkillCDReduction(GameContextRecord context, ObjectId targetd, int baseValue, List<int> skillIds) 
        {
            CharacterContext characters = context.CharacterRecord;
            WorldContext world = context.WorldRecord;
            CharacterState state = characters.MemberDataContainer[targetd].PlayerState;


            foreach (int id in skillIds) 
            {
                int cd = state.CurrentSkillCooldowns[id];
                state.CurrentSkillCooldowns[id] = Mathf.Max(0, cd - baseValue);
            }

            GameContextRecord newContext = new GameContextRecord(world, characters);
            return newContext;
        }


        public static GameContextRecord MoveTo(
            GameContextRecord context,
            ObjectId movingTargetId,
            Vector3Int destination,
            Vector3Int origin)
        {
            CharacterContext characters = context.CharacterRecord; 
            WorldContext world = context.WorldRecord;

            // Moving the tile on the board state
            Vector3Int o = world.WorldMap.LocalToCell(origin);
            world.UpdateBoardState(destination, movingTargetId);
            world.RemoveBoardPiece(o, movingTargetId);

            CharacterState state = characters.MemberDataContainer[movingTargetId].PlayerState;
            state.Position = destination;

            CharacterStateContainer container = new CharacterStateContainer(
                characters.MemberDataContainer[movingTargetId].PlayerTransform, state);

            characters.MemberDataContainer[movingTargetId] = container;

            GameContextRecord gameContext = new GameContextRecord(world, characters);
            return gameContext;
        }

        public static GameContextRecord Combat(
            GameContextRecord context, 
            ObjectId attackerId,
            ObjectId targetId
            )
        {
            CharacterContext characters = context.CharacterRecord;
            WorldContext world = context.WorldRecord;

            CharacterState attacker = characters.MemberDataContainer[attackerId].PlayerState;
            CharacterState target = characters.MemberDataContainer[targetId].PlayerState;

            // Deal damage to the target.
            GameContextRecord gameContext = DealDamage(context, targetId, attacker.CurrentAttack);
            Vector3Int retreatDestination = GetClassKnockbackPattern(attacker.Character.ClassType, attacker.Position, target.Position);
            ObjectId defender = world.GetOccupyingTileAt(retreatDestination);
            bool isRetreatImpossible = TargetEvaluation.IsRetreatDenied(targetId, defender, retreatDestination, world);
            // If the retreating destination has another ally unit to pincer attack(not the attacker):
            if (defender != attackerId && isRetreatImpossible)
            {
                gameContext = DefeatTarget(gameContext, targetId);
                return MoveTo(gameContext, attacker.ObjectId, target.Position, attacker.Position);
            }
            
            // Retreat to the free destination with attacker occupying previous position of retreating character.

            // Move attacker
            gameContext = MoveTo(gameContext, attackerId, target.Position, attacker.Position);
            
            // Move retreating character
            gameContext = MoveTo(gameContext, targetId, retreatDestination, target.Position);

            // Attacker moves to occupy the attacked position.
            return gameContext;
        }

        public static GameContextRecord DefeatTarget(GameContextRecord context, ObjectId defeatedId) 
        {
            WorldContext world = context.WorldRecord;
            CharacterContext character = context.CharacterRecord;
            CharacterState defeated = character.MemberDataContainer[defeatedId].PlayerState;
            defeated.IsActive = false;
            world.RemoveBoardPiece(defeated.Position, defeatedId);
            return new GameContextRecord(world, character);
        }

        public static Vector3Int GetClassKnockbackPattern(CharacterClassType attackerClass, Vector3Int attackerPos, Vector3Int targetPos)
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
                    knockbackPos = GetKnockbackPosition(targetPos, direction, 2);
                    break;
                default:
                    knockbackPos = GetKnockbackPosition(targetPos, direction, 1);
                    break;
            }

            return knockbackPos;
        }

        public static Vector3Int GetKnockbackPosition(Vector3Int targetPosition, Vector3Int direction, int knockbackScale)
        {
            return targetPosition + knockbackScale * direction;
        }
    } 
}


