using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class ControllableTileManager : TileManager
    {
        Dictionary<TileId, ControllableTileContainer> m_players = new Dictionary<TileId, ControllableTileContainer>();


        public override void Init()
        {
        }


        public TileBehaviour Player(TileId id)
        {
            return m_players[id].Player.Behaviour;
        }

        public override GameObject SpawnTile(TileId id, GameObject objectRef, Vector3 position, Transform parent, bool isActive = true) 
        {
            GameObject o = m_spawner.SpawnTile(id, objectRef, position, parent, isActive);
            TileItem item = new TileItem { TileObject = o, Behaviour = o.GetComponent<TileBehaviour>() };

            ControllableTileContainer newPlayer = new ControllableTileContainer(item);

            m_players.Add(id, newPlayer);

            return o;
        }

        public override void DespawnTiles(TileId id)
        {
            if (id == null)
            {
                return;
            }

            UnsubscribeToTileEvents(m_players[id]);
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
            SubscribeToTileEvents(m_players[id]);
        }

        public override void HideTile(TileId id)
        {
            if (id == null)
            {
                return;
            }
            m_players[id].HideTiles();
            UnsubscribeToTileEvents(m_players[id]);
        }

        public override void MoveTile(TileId id, Vector3 offset)
        {
            if (id == null)
            {
                return;
            }

            m_players[id].MoveTiles(offset);
        }


        public override void Shutdown()
        {
            base.Shutdown();
        }
    }

}

