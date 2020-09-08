using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class CharacterTileManager : TileManager
    {
        Dictionary<TileId, ControllableTileContainer> m_players = new Dictionary<TileId, ControllableTileContainer>();
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
            RangeDisplay rangeDisplay = null,
            bool isActive = true) 
        {
            GameObject o = m_spawner.SpawnTile(id, objectRef, position, parent, charData, rangeDisplay, isActive);
            TileItem item = new TileItem { TileObject = o, Behaviour = o.GetComponent<TileBehaviour>() };

            ControllableTileContainer newPlayer = new ControllableTileContainer(item);

            m_players.Add(id, newPlayer);

            return o;
        }

        public void SelectCharacter(TileId id) 
        { 
            if( id == null || !m_players.ContainsKey(id)) 
            {
                return;
            }

            m_players[id].SelectTile();
        }

        public void UnselectCharacter(TileId id)
        {
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].UnselectTile();
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
    }

}

