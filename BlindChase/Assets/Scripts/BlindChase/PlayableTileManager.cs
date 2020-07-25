using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class PlayableTileManager : TileManager
    {

        Dictionary<string, PlayableTileContainer> m_players = new Dictionary<string, PlayableTileContainer>();
        public TileBehaviour Player(string id) 
        {
            return m_players[id].Player.Behaviour;
        }

        public override GameObject SpawnTile(string id, GameObject objectRef, Vector3 position, Transform parent, bool isActive = true) 
        {
            GameObject o = m_spawner.SpawnTile(objectRef, position, parent, isActive);
            TileItem item = new TileItem { TileObject = o, Behaviour = o.GetComponent<TileBehaviour>() };

            PlayableTileContainer newPlayer = new PlayableTileContainer(item);

            m_players.Add(id, newPlayer);

            return o;
        }

        public override void DespawnTiles(string id) 
        {
            m_players[id].DestroyTiles();
        }

        public override void ShowTile(string id)
        {
            m_players[id].ShowTiles();
            m_players[id].OnTileTrigger += OnTileEventTriggered;
        }

        public override void HideTile(string id)
        {
            m_players[id].HideTiles();
            m_players[id].OnTileTrigger -= OnTileEventTriggered;

        }

        public override void MoveTile(string id, Vector3 offset)
        {
            m_players[id].MoveTiles(offset);
        }
    }

}

