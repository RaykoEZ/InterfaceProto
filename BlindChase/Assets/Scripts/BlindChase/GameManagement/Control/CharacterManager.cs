using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Animation;
using BlindChase.Events;
using BlindChase.UI;

namespace BlindChase.GameManagement 
{
    public class CharacterManager : TileManager
    {
        [SerializeField] Tilemap m_map = default;
        static Dictionary<ObjectId, CharacterContainer> m_players = new Dictionary<ObjectId, CharacterContainer>();

        public override void Init()
        {
        }

        public TileBehaviour Player(ObjectId id)
        {
            return m_players[id].Player.Behaviour;
        }

        public override GameObject SpawnTile(
            ObjectId id, GameObject objectRef, Vector3 position, Transform parent,
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

        public override void DespawnTiles(ObjectId id)
        {
            if (id == null)
            {
                return;
            }
            
            m_players[id].DestroyTiles();
            m_players.Remove(id);
        }

        public override void ShowTile(ObjectId id)
        {
            if (id == null)
            {
                return;
            }

            m_players[id].ShowTiles();
        }

        public override void HideTile(ObjectId id)
        {
            if (id == null)
            {
                return;
            }
            m_players[id].HideTiles();
        }

        public override void MoveTile(ObjectId id, Vector3 offset)
        {
            MoveCharacter(id, offset);
        }

        public static void MoveCharacter(ObjectId id, Vector3 offset) 
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


        public void OnCharacterAdvance(ObjectId id, Vector3Int destCoord, Action onFinish, List<Action<bool>> onHit = null)
        {
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            Vector3 dest = m_map.GetCellCenterWorld(destCoord);
            MotionDetail detail = new MotionDetail(dest);
            m_players[id].Player.Behaviour?.OnAdvance(detail, onFinish, onHit);
        }
        public void OnCharacterTakeDamage(ObjectId id, Vector3Int destCoord, Action onFinish, bool isLastHit)
        {
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            Vector3 dest = m_map.GetCellCenterWorld(destCoord);
            MotionDetail detail = new MotionDetail(dest);
            m_players[id].Player.Behaviour?.OnTakeDamage(detail, onFinish, isLastHit);
        }

        // Impl

        public void OnCharacterDefeated(EventInfo info)
        {
            ObjectId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].Player.Behaviour?.OnSelfDefeated(info);
        }

        // Impl
        public void OnCharacterSkillActivate(ObjectId id)
        {
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].Player.Behaviour?.OnSkillActivate();
        }

        // Impl
        public void OnLeaderDefeated(EventInfo info)
        {
            ObjectId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }
            m_players[id].Player.Behaviour?.OnLeaderDefeated(info);
        }

        // Impl
        // Some animation when a character is selected.
        public void OnCharacterSelected(EventInfo info)
        {
            ObjectId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].Player.Behaviour?.OnSelect();
        }

        // Impl
        public void OnCharacterUnselected(EventInfo info)
        {
            ObjectId id = info.SourceId;
            if (id == null || !m_players.ContainsKey(id))
            {
                return;
            }

            m_players[id].Player.Behaviour?.OnUnselect();
        }
    }

}

