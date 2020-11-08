using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.Animation;

namespace BlindChase.GameManagement
{
    public class CharacterBehaviour : TileBehaviour
    {
        [SerializeField] AnimatorHelper m_animatorHelper = default;

        List<Action<bool>> m_onAdvanceTriggers = new List<Action<bool>>();

        public override void Init(ObjectId tileId, CharacterData characterData)
        {
            base.Init(tileId, characterData);
            m_animatorHelper.Init(characterData.CharacterId);
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

        public void OnAdvance(MotionDetail motion, Action onFinish, List<Action<bool>> onAdvanceTriggers = null)
        {
            if(onAdvanceTriggers != null) 
            {
                m_onAdvanceTriggers.AddRange(onAdvanceTriggers);
            }

            m_animatorHelper.TriggerAnimation(CommandTypes.ADVANCE, "OnCharacterAdvance", onFinish, motion);
        }

        public void OnAdvanceAttackFrame(int isLastHit) 
        {
            bool isLastHitBool = Convert.ToBoolean(isLastHit);
            if(m_onAdvanceTriggers.Count == 0) 
            {
                return;
            }

            foreach(Action<bool> trigger in m_onAdvanceTriggers) 
            {
                trigger?.Invoke(isLastHitBool);
            }

            if (isLastHitBool) 
            {
                m_onAdvanceTriggers.Clear();
            }
        }

        public void OnTakeDamage(MotionDetail motion, Action onFinish, bool isLastHit)
        {
            // If we react to take damage for the last time this turn, trigger onFinish callback
            if (isLastHit) 
            {
                // Final take damage animation calls
                m_animatorHelper.TriggerAnimation("OnTakeDamage", "OnTakeDamage", OnFinished: onFinish, motion: motion);
            }
            else 
            {
                // Intermediate take damage animation calls
                m_animatorHelper.TriggerAnimation("OnTakeDamage", "OnTakeDamage");
            }

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

    }

}
