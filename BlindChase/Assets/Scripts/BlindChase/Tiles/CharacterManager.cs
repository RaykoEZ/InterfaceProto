using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;


namespace BlindChase 
{
    public class CharacterManager : TileManager
    {
        [SerializeField] Tilemap m_map = default;
        static Dictionary<TileId, CharacterContainer> m_players = new Dictionary<TileId, CharacterContainer>();

        public override void Init()
        {
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
            CharacterItem item = new CharacterItem { TileObject = o, Behaviour = o.GetComponent<CharacterBehaviour>()};
            CharacterContainer newPlayer = new CharacterContainer(item);
            
            m_players.Add(id, newPlayer);

            return o;
        }

        public override void DespawnTiles(TileId id)
        {
            if (id == null)
            {
                return;
            }
            
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
        }

        public override void HideTile(TileId id)
        {
            if (id == null)
            {
                return;
            }
            m_players[id].HideTiles();
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
            m_players[id].Player.Behaviour?.OnSkillActivate(info);

        }

        public void OnCharacterAdvance(TileId id, Vector3Int destCoord, Action onFinish, List<Action<bool>> onHit = null)
        {
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            Vector3 dest = m_map.GetCellCenterWorld(destCoord);
            MotionDetail detail = new MotionDetail(dest);
            m_players[id].Player.Behaviour?.OnAdvance(detail, onFinish, onHit);
        }
        public void OnCharacterTakeDamage(TileId id, Vector3Int destCoord, Action onFinish, bool isLastHit)
        {
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            Vector3 dest = m_map.GetCellCenterWorld(destCoord);
            MotionDetail detail = new MotionDetail(dest);
            m_players[id].Player.Behaviour?.OnTakeDamage(detail, onFinish, isLastHit);
        }

        public void OnCharacterDefeated(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].Player.Behaviour?.OnSelfDefeated(info);

        }

        public void OnLeaderDefeated(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].Player.Behaviour?.OnLeaderDefeated(info);
        }



        public void OnCharacterSelected(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].Player.Behaviour?.OnSelect();
        }

        public void OnCharacterUnselected(EventInfo info)
        {
            TileId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].Player.Behaviour?.OnUnselect();
        }
    }

}

