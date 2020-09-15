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

        TileId m_focusedTileId;
        CharacterContext m_characterContext;

        public void Init(
            TurnOrderManager turnOrder,
            CharacterContextFactory characterContext)
        {
            // Whenever turn starts, focus onto the active character
            turnOrder.OnCharacterTurnStart += ChangeFocusCharacter;
            // Whenever player selects a character, focus on that character
            m_tileSelector.OnTileSelect += ChangeFocusCharacter;
            characterContext.OnContextChanged += OnCharacterContextUpdate;
            m_characterContext = characterContext.Context;
            m_HUD.OnSkillClick += OnSkillSelect;

        }

        void OnCharacterContextUpdate(CharacterContext context) 
        {
            m_characterContext = context;
            UpdateCharacterStateDisplay(m_focusedTileId);
        }

        void UpdateCharacterStateDisplay(TileId id) 
        {
            m_focusedTileId = id;
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

            m_HUD.LoadValues(state);
            m_HUD.LoadSkillData(
                skillIds,
                skillLevels,
                skillCooldowns,
                skillDataCollection,
                sprites
                );
        }

        void ChangeFocusCharacter(TileId id) 
        {
            m_camera.FocusCamera(m_characterContext.MemberDataContainer[id].PlayerTransform.position);
            UpdateCharacterStateDisplay(id);
        }

        void OnSkillSelect(int skillId, int skillLevel) 
        {
            CharacterStateContainer characterData = m_characterContext.MemberDataContainer[m_focusedTileId];

            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["SkillId"] = skillId;
            payload["SkillLevel"] = skillLevel;

            CommandEventInfo info = new CommandEventInfo(
                characterData.PlayerState.TileId,
                CommandTypes.SKILL_PROMPT,
                payload);

            OnSkillPrompt?.TriggerEvent(info);
        }
    }

}


