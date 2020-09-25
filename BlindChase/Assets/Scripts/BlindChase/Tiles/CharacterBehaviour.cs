using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.Animation;
using TMPro;

namespace BlindChase 
{
    public class CharacterBehaviour : TileBehaviour
    {
        // Test display
        //[SerializeField] TextMeshPro m_idText = default;
        [SerializeField] AnimatorSetupHelper m_animSetupHelper = default;
        
        public override void Init(TileId tileId, CharacterData characterData)
        {
            base.Init(tileId, characterData);

            //m_idText.text = $"{m_tileId.FactionId}/{m_tileId.UnitId}";

            m_animSetupHelper.Init(characterData.CharacterId);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void OnSelect()
        {
        }

        public override void OnUnselect()
        {
        }

        public void OnAttack(EventInfo info)
        {
            Debug.Log("Attack!!!");
        }

        public void OnSkillActivate(EventInfo info)
        {
            Debug.Log("Skill Activated");
        }
        public void OnSelfDefeated(EventInfo info)
        {
            Debug.Log("Defeated");
        }

        public void OnLeaderDefeated(EventInfo info)
        {
            Debug.Log("Defeated");
        }
        public void OnTakeDamage(EventInfo info)
        {
            Debug.Log("Owwww");

        }

        public virtual void OnPlayerSkillSelect(string rangeId, int targetLimit) 
        {
        }

        public virtual void OnRangeTileTrigger(CommandEventInfo eventArg)
        {
            Dictionary<string, object> payload = eventArg.Payload;
            if (payload == null) 
            {
                payload = new Dictionary<string, object>();
            }

            payload.Add("Origin", transform.position);
            CommandEventInfo overrideArg = new CommandEventInfo(m_tileId, eventArg.CommandType, payload);
            m_onTileCommand?.Invoke(overrideArg);
        }
     
    }

}
