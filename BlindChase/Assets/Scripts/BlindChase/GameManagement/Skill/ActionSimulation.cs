using UnityEngine;

namespace BlindChase.GameManagement
{
    // Used to obtain results for character actions without directly modifying current game state.
    public static class ActionSimulation
    {
        public static CharacterState DealDamage(int baseValue, ref CharacterState targetState)
        {
            Debug.Log("Damage");
            int netDamage = Mathf.Max(baseValue - targetState.CurrentDefense, 0);
            targetState.CurrentHP -= netDamage;

            return targetState;
        }


        public static CharacterState RestoreHP(int baseValue, ref CharacterState targetState)
        {
            Debug.Log($"Healing for {baseValue} HP.");

            targetState.CurrentHP = Mathf.Min(targetState.CurrentHP + baseValue, targetState.Character.MaxHP);

            return targetState;
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

            Vector3Int retreatDestination = GetClassKnockbackPattern(attacker.Character.ClassType, attacker.Position, target.Position);
            ObjectId occupier = world.GetOccupyingTileAt(retreatDestination);
            GameContextRecord gameContext = new GameContextRecord(world, characters);

            // If the retreating destination has another ally unit to pincer attack(not the attacker):
            if (occupier != null && occupier != attackerId)
            {
                DefeatTarget(target, world);
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

        public static void DefeatTarget(CharacterState characterState, WorldContext world) 
        {
            Debug.Log($"Character: {characterState.Character.Name} Defeated!");
            characterState.IsActive = false;
            world.RemoveBoardPiece(characterState.Position, characterState.ObjectId);
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


