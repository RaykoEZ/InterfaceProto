using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.GameManagement;
using BlindChase.Events;

namespace BlindChase.Ui
{
    public class GameplayScreen : MonoBehaviour
    {
        [SerializeField] CameraManager m_camera = default;
        [SerializeField] CharacterHUD m_HUD = default;
        [SerializeField] SkillDatabase m_skillDatabase = default;
        [SerializeField] TileSelectionManager m_tileSelector = default;
        [SerializeField] BCGameEventTrigger OnSkillPrompt = default;

        ObjectId m_focusedCharacterId;
        // The character that can execute commands right now.
        ObjectId m_activeCharacterId;
        CharacterContext m_characterContext;

        public void Init(
            TurnOrderManager turnOrder,
            CharacterContextFactory characterContext)
        {
            // Whenever turn starts, focus onto the active character
            turnOrder.OnTurnStart += OnTurnStart;

            // Whenever player selects a character, focus on that character
            m_tileSelector.OnTileSelected += OnTileSelection;
            characterContext.OnContextChanged += OnCharacterContextUpdate;
            m_characterContext = characterContext.Context;
            m_HUD.OnSkillClick += OnSkillSelect;

        }

        void OnCharacterContextUpdate(CharacterContext context) 
        {
            m_characterContext = context;
            UpdateCharacterStateDisplay(m_activeCharacterId);
        }

        void UpdateCharacterStateDisplay(ObjectId id) 
        {
            m_focusedCharacterId = id;
            CharacterState state = m_characterContext.MemberDataContainer[id].PlayerState;
            List<IdLevelPair> skillLevels = state.Character.SkillLevels;
            List<SkillSlotData> skillslotData = new List<SkillSlotData>();
            foreach (IdLevelPair skillLevel in skillLevels)
            {
                SkillDataCollection skill = SkillManager.GetSkillData(skillLevel.Id);
                int cooldown = state.CurrentSkillCooldowns[skillLevel.Id];
                Sprite skillIcon = m_skillDatabase.GetSkillIcon(skillLevel.Id);
                SkillSlotData slotData = new SkillSlotData(skillLevel.Id, skillLevel.Level, cooldown, skillIcon, skill);
                skillslotData.Add(slotData);
            }

            bool isPreview = id != m_activeCharacterId || !string.IsNullOrEmpty(id.NPCId);

            m_HUD.LoadValues(state);

            m_HUD.LoadSkillData(
                skillslotData,
                isPreview
                );
        }

        void ChangeFocusCharacter(ObjectId id) 
        {
            m_camera.FocusCamera(m_characterContext.MemberDataContainer[id].PlayerTransform.position);
            UpdateCharacterStateDisplay(id);
        }

        public void FocusOnActiveCharacter(EventInfo info)
        {
            bool isMoving = (bool)info.Payload["isMoving"];
            if (isMoving) 
            {
                ChangeFocusCharacter(m_activeCharacterId);
            }
        }

        void OnTurnStart(ObjectId id) 
        {
            m_activeCharacterId = id;
            ChangeFocusCharacter(id);
        }

        void OnTileSelection(EventInfo info) 
        { 
            if(info.SourceId != null) 
            {
                ChangeFocusCharacter(info.SourceId);
            }
            else 
            {
                m_camera.FocusCamera((Vector3)info.Payload["Destination"]);
            }
        }

        void OnSkillSelect(int skillId, int skillLevel) 
        {
            CharacterStateContainer characterData = m_characterContext.MemberDataContainer[m_focusedCharacterId];

            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["SkillId"] = skillId;
            payload["SkillLevel"] = skillLevel;

            EventInfo info = new EventInfo(
                characterData.PlayerState.ObjectId,
                payload);

            m_camera.FocusCamera(characterData.PlayerTransform.position);
            OnSkillPrompt?.TriggerEvent(info);
        }
    }

}


