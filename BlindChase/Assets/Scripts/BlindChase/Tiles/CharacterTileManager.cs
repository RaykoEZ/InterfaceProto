using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class CharacterTileManager : TileManager
    {
        static Dictionary<TileId, CharacterTileContainer> m_players = new Dictionary<TileId, CharacterTileContainer>();
        public event OnCharacterTileActivate OnPlayerSelect = default;
        TileId m_commandTarget;

        public override void Init(TurnOrderManager turnOrderManager)
        {
            turnOrderManager.OnCharacterTurnStart += OnCommandTargetChange;
        }

        protected void OnCommandTargetChange(TileId id) 
        {
            if (m_commandTarget != null)
            {
                m_players[m_commandTarget].DisallowPlayerCommand();
                UnsubscribeToCommand(m_players[m_commandTarget]);
            }

            m_players[id].AllowPlayerCommand();
            SubscribeToCommand(m_players[id]);
            m_commandTarget = id;
        }

        public TileBehaviour Player(TileId id)
        {
            return m_players[id].Player.Behaviour;
        }

        public override GameObject SpawnTile(
            TileId id, GameObject objectRef, Vector3 position, Transform parent,
            CharacterData charData = default,
            PromptHandler rangeDisplay = null,
            bool isActive = true) 
        {
            GameObject o = m_spawner.SpawnTile(id, objectRef, position, parent, charData, isActive);
            CharacterTileItem item = new CharacterTileItem { TileObject = o, Behaviour = o.GetComponent<CharacterBehaviour>()};
            CharacterTileContainer newPlayer = new CharacterTileContainer(item);
            
            m_players.Add(id, newPlayer);

            return o;
        }

        protected void OnPlayerSelectCharacter(TileId id) 
        {
            OnPlayerSelect?.Invoke(id);
        }

        public override void DespawnTiles(TileId id)
        {
            if (id == null)
            {
                return;
            }
            
            UnsubscribeToSelection(m_players[id]);
            m_players[id].DestroyTiles();
            m_players.Remove(id);
        }

        public override void ShowTile(TileId id)
        {
            if (id == null)
            {
                return;
            }

            m_players[id].ShowTiles();
            SubscribeToSelection(m_players[id]);
        }

        public override void HideTile(TileId id)
        {
            if (id == null)
            {
                return;
            }
            m_players[id].HideTiles();
            UnsubscribeToSelection(m_players[id]);
        }

        public override void MoveTile(TileId id, Vector3 offset)
        {
            MoveCharacter(id, offset);
        }

        public static void MoveCharacter(TileId id, Vector3 offset) 
        {
            if (id == null)
            {
                return;
            }

            m_players[id].MoveTiles(offset);
        }

        protected void SubscribeToSelection(TileContainer container)
        {
            container.OnPlayerSelect += OnPlayerSelectCharacter;
        }

        protected void UnsubscribeToSelection(TileContainer container)
        {
            container.OnPlayerSelect -= OnPlayerSelectCharacter;
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }


        public void OnCharacterSkillActivate(EventInfo info) 
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].OnCharacterSkillActivate(info);

        }

        public void OnCharacterAttack(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].OnCharacterAttack(info);

        }

        public void OnCharacterDefeated(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].OnCharacterDefeated(info);

        }

        public void OnLeaderDefeated(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].OnLeaderDefeated(info);
        }

        public void OnCharacterTakeDamage(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].OnCharacterTakeDamage(info);
        }

        public void OnCharacterSelected(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].SelectCharacter();
        }

        public void OnCharacterUnselected(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].UnselectCharacter();
        }
    }

}

