using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;
using TMPro;


namespace BlindChase 
{
    public class CharacterBehaviour : TileBehaviour
    {
        // Test display
        [SerializeField] TextMeshPro m_idText = default;
        [SerializeField] Tilemap m_localMap = default;
        [SerializeField] Transform m_rangeTileParent = default;

        static TileId m_commandingTileId = default;
        public override void Init(TileId tileId, RangeDisplay rangeDisplay, CharacterData characterData)
        {
            base.Init(tileId, rangeDisplay, characterData);

            m_idText.text = $"{m_tileId.FactionId}/{m_tileId.UnitId}";
        }
        public override void OnStartCommand() 
        {
            m_rangeDisplayRef.OnRangeTileEvent += OnRangeTileTrigger;
            m_commandingTileId = m_tileId;
        }
        public override void OnFinishCommand()
        {
            m_rangeDisplayRef.OnRangeTileEvent -= OnRangeTileTrigger;
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void OnPlayerPointerSelect()
        {
            if (m_commandingTileId == m_tileId) 
            {
                ShowMovementRange(true);
            }
            else 
            {
                ShowMovementRangePreview(true);
            }

            m_onTileSelect?.Invoke(m_tileId);
        }

        public override void OnPlayerPointerUnselect()
        {
            m_rangeDisplayRef.HideAll();
        }


        public virtual void OnPlayerSkillSelect(string rangeId, int targetLimit) 
        {
            TileId tileId = new TileId(
                CommandTypes.SKILL_ACTIVATE,
                m_tileId.FactionId,
                m_tileId.UnitId);
            m_rangeDisplayRef.ShowSkillTargetOption(
                tileId, rangeId, transform.position, targetLimit, m_localMap, m_rangeTileParent);
        }

        void ShowMovementRange(bool toggle = false) 
        {
            TileId tileId = new TileId(
                    CommandTypes.MOVE,
                    m_tileId.FactionId,
                    m_tileId.UnitId);
            m_rangeDisplayRef.ShowRangeMap(
                tileId, m_charData.ClassType, transform.position, m_localMap, m_rangeTileParent, toggle);
        }

        void ShowMovementRangePreview(bool toggle = false)
        {
            TileId tileId = new TileId(
                    CommandTypes.MOVE_PROMPT,
                    m_tileId.FactionId,
                    m_tileId.UnitId);
            m_rangeDisplayRef.ShowRangeMapPreview(
                tileId, m_charData.ClassType, transform.position, m_localMap, m_rangeTileParent, toggle);
        }

        public virtual void OnRangeTileTrigger(CommandEventInfo eventArg)
        {
            Dictionary<string, object> payload = eventArg.Payload;
            if (payload == null) 
            {
                payload = new Dictionary<string, object>();
            }

            payload.Add("Origin", transform.position);
            CommandEventInfo overrideArg = new CommandEventInfo(m_tileId, eventArg.Location, eventArg.CommandType, payload);
            m_onTileCommand?.Invoke(overrideArg);
        }
    }

}
