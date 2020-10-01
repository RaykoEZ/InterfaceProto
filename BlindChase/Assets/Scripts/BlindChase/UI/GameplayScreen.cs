using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.Events;

namespace BlindChase
{
    public class GameplayScreen : MonoBehaviour
    {
        [SerializeField] CameraManager m_camera = default;
        [SerializeField] CharacterHUD m_HUD = default;
        [SerializeField] SkillDatabase m_skillDatabase = default;
        [SerializeField] TileSelectionManager m_tileSelector = default;
        [SerializeField] BCGameEventTrigger OnSkillPrompt = default;

        TileId m_focusedCharacterId;
        // The character that can execute commands right now.
        TileId m_activeCharacterId;
        CharacterContext m_characterContext;

        public void Init(
            TurnOrderManager turnOrder,
            CharacterContextFactory characterContext)
        {
            // Whenever turn starts, focus onto the active character
            turnOrder.OnCharacterTurnStart += OnTurnStart;
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

        void UpdateCharacterStateDisplay(TileId id) 
        {
            m_focusedCharacterId = id;
            CharacterState state = m_characterContext.MemberDataContainer[id].PlayerState;

            List<int> skillIds = state.Character.SkillIds;
            List<int> skillLevels = state.Character.SkillLevels;

            List<int> skillCooldowns = new List<int>();
            List<SkillDataCollection> skillDataCollection = new List<SkillDataCollection>();
            List<Sprite> sprites = new List<Sprite>();

            foreach (int skillId in skillIds)
            {
                SkillDataCollection skill = m_skillDatabase.GetSkill(skillId);
                skillDataCollection.Add(skill);
                skillCooldowns.Add(state.CurrentSkillCooldowns[skillId]);
                sprites.Add(m_skillDatabase.GetSkillIcon(skillId));  
            }

            bool isPreview = id != m_activeCharacterId;

            m_HUD.LoadValues(state);
            m_HUD.LoadSkillData(
                skillIds,
                skillLevels,
                skillCooldowns,
                skillDataCollection,
                sprites,
                isPreview
                );
        }

        void ChangeFocusCharacter(TileId id) 
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

        void OnTurnStart(TileId id) 
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

            CommandEventInfo info = new CommandEventInfo(
                characterData.PlayerState.TileId,
                CommandTypes.SKILL_PROMPT,
                payload);

            m_camera.FocusCamera(characterData.PlayerTransform.position);
            OnSkillPrompt?.TriggerEvent(info);
        }
    }

}


