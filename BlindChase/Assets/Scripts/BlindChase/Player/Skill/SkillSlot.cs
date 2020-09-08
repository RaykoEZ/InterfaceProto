using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlindChase;

namespace BlindChase
{
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField] Button m_skillIcon = default;
        [SerializeField] TextMeshProUGUI m_cooldownCount = default;
        public event OnSkillClicked OnSkillClick = default;
        
        int m_skillId;
        int m_skillLevel;
        string m_skillName;
        string m_skillDescription;
        Sprite m_skillSprite;
        bool m_isPreview;
        public void LoadValues(
            int skillId, 
            int skillLevel,
            int skillCooldown,
            string skillName,
            string skillDescription,
            Sprite skillIcon,
            bool isPreview = false)
        {
            m_skillId = skillId;
            m_skillLevel = skillLevel;
            m_skillName = skillName;
            m_skillDescription = skillDescription;
            m_skillSprite = skillIcon;
            m_skillIcon.image.sprite = m_skillSprite;
            m_isPreview = isPreview;
            OnCooldown(skillCooldown);
        }

        public void OnCooldown(int skillCooldown)
        {
            bool usable = skillCooldown == 0;
            m_skillIcon.interactable = usable;
            m_cooldownCount.text = skillCooldown.ToString();
            m_cooldownCount.gameObject.SetActive(usable);
        }

        public void OnSkillClicked() 
        {
            OnSkillClick?.Invoke(m_skillId, m_skillLevel);
        }
    }

}


