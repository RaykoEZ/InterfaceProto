using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlindChase;

namespace BlindChase
{
    // This class populates the given character status into the correct display fields.
    public class CharacterHUD : UIBehaviour
    {
        [SerializeField] Image m_characterImage = default;
        [SerializeField] Image m_characterClassIcon = default;

        [SerializeField] TextMeshProUGUI m_characterName = default;
        [SerializeField] TextMeshProUGUI m_characterHP = default;
        [SerializeField] TextMeshProUGUI m_CharacterSP = default;
        [SerializeField] TextMeshProUGUI m_CharacterDef = default;
        [SerializeField] TextMeshProUGUI m_CharacterSpeed = default;
        [SerializeField] List<SkillSlot> m_skillSlots = new List<SkillSlot>();
        [SerializeField] Button m_confirmSkill = default;
        [SerializeField] Button m_cancelSkill = default;

        [SerializeField] Toggle m_movementToggle = default;

        bool m_skillTargetInProgress = false;

        public event OnSkillClicked OnSkillClick = default;
        public event OnSkillCancelled OnRangeCancel = default;
        public event OnSkillConfirmed OnRangeConfirm = default;


        void Start()
        {
            foreach (SkillSlot slot in m_skillSlots)
            {
                slot.OnSkillClick += OnSkillSlotClicked;
            }
        }

        void OnDestroy()
        {
            OnRangeCancel = null;
            foreach (SkillSlot slot in m_skillSlots)
            {
                slot.OnSkillClick -= OnSkillSlotClicked;
            }
        }

        void OnSkillSlotClicked(int skillId, int skillLevel) 
        {
            m_skillTargetInProgress = true;
            OnSkillClick?.Invoke(skillId, skillLevel);
            m_confirmSkill.interactable = false;
            m_cancelSkill.interactable = true;
        }

        public void SkillTargetSatisfied() 
        {
            m_confirmSkill.interactable = true;
        }

        public void CancelOption()
        {
            ResetSkillButtons(false);
            OnRangeCancel?.Invoke();
        }
        public void ConfirmOption()
        {
            ResetSkillButtons(false);
            m_skillTargetInProgress = false;
            OnRangeConfirm?.Invoke();
        }

        void ResetSkillButtons(bool value) 
        {
            m_confirmSkill.interactable = value;
            m_cancelSkill.interactable = value;
        }

        public void LoadValues(CharacterState characterState)
        {
            m_characterName.text = characterState.Character.Name;
            m_characterHP.text = $"HP: {characterState.CurrentHP} / {characterState.Character.MaxHP}";
            m_CharacterSP.text = $"SP: {characterState.CurrentSP} / {characterState.Character.MaxSP}";
            m_CharacterDef.text = $"Def: {characterState.CurrentDefense}";
            m_CharacterSpeed.text = $"Speed: {characterState.CurrentSpeed}";
        }

        public void LoadSkillData(
            List<int> skillIds, 
            List<int> skillLevels,
            List<int> skillCooldown,
            List<SkillDataCollection> skillData,
            List<Sprite> skillIcons,
            bool isPreview = false
            ) 
        {
            if (m_skillTargetInProgress) 
            {
                return;
            }

            if (skillIds.Count > skillLevels.Count ||
                skillIds.Count > skillData.Count ||
                skillIds.Count > skillCooldown.Count)
            {
                Debug.LogError("Not enough skill data provided to skill slots.");
                return;
            }

            // We shutdown the button interactions if it's preview-only.
            ResetSkillButtons(false);
            m_movementToggle.interactable = !isPreview;

            foreach (SkillSlot slot in m_skillSlots) 
            {
                slot.gameObject.SetActive(false);
            }
            
            for (int i = 0; i< skillIds.Count; ++i) 
            {
                int skillId = skillIds[i];
                int skillLevel = skillLevels[i];
                int cooldown = skillCooldown[i];
                string skillName = skillData[i].Name;
                string skillDescription = skillData[i].Text;
                Sprite sprite = skillIcons[i];
                m_skillSlots[i].gameObject.SetActive(true);
                m_skillSlots[i].LoadValues(skillId, skillLevel, cooldown, skillName, skillDescription, sprite, isPreview);
            }           

        }
    }

}


