using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlindChase.Events;

namespace BlindChase.UI
{
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField] Button m_skillIcon = default;
        [SerializeField] TextMeshProUGUI m_cooldownCount = default;
        public event OnSkillClicked OnSkillClick = default;
        
        int m_skillId;
        int m_skillLevel;
        int m_coolDown;
        string m_skillName;
        string m_skillDescription;
        Sprite m_skillSprite;

        bool m_isPreview = false;
        bool m_usable = false;
        public void LoadValues(
            SkillSlotData slotData,
            bool isPreview = false)
        {
            m_skillId = slotData.SkillId;
            m_skillLevel = slotData.SkillLevel;
            m_skillName = slotData.SkillData.Name;
            m_skillDescription = slotData.SkillData.Text;
            m_skillSprite = slotData.SkillIcon;
            m_skillIcon.image.sprite = m_skillSprite;
            m_coolDown = slotData.Cooldown;

            m_isPreview = isPreview;
            m_usable = m_coolDown == 0;

            OnCooldown();
        }

        void OnCooldown() 
        {

            if (!m_usable) 
            {
                m_skillIcon.image.color = Color.grey;
                m_cooldownCount.text = m_coolDown.ToString();
            }
            else 
            {
                m_skillIcon.image.color = Color.white;
            }

            m_cooldownCount.enabled = !m_usable;
        }

        public void OnSkillClicked() 
        {
            if (!m_isPreview && m_usable) 
            {
                OnSkillClick?.Invoke(m_skillId, m_skillLevel);
            }
            else 
            {
                Debug.Log($"Skill not usable right now. Cooldown: {m_coolDown} turns.");
            }
        }
    }

}


