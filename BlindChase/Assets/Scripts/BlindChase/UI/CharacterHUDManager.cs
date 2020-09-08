using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.Events;

namespace BlindChase
{
    public class CharacterHUDManager : MonoBehaviour
    {
        public event OnPlayerCommand<CommandEventInfo> OnPlayerCommand = default;
        [SerializeField] CameraManager m_camera = default;
        [SerializeField] CharacterHUD m_HUD = default;
        [SerializeField] SkillDatabase m_skillDatabase = default;
        [SerializeField] Transform m_actionOption = default;
        [SerializeField] TileSelectionManager m_tileSelector = default;
        TileId m_focusedTileId;
        CharacterContext m_characterContext;

        private void OnDestroy()
        {
            OnPlayerCommand = null;
        }

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
        }

        void OnCharacterSelection(TileId id) 
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

            HideHUD();

            m_HUD.LoadValues(state);
            m_HUD.LoadSkillData(
                skillIds,
                skillLevels,
                skillCooldowns,
                skillDataCollection,
                sprites
                );

            ShowHUD();
        }

        void ChangeFocusCharacter(TileId id) 
        {
            m_camera.FocusCamera(m_characterContext.MemberDataContainer[id].PlayerTransform.position);

            OnCharacterSelection(id);
        }

        void OnSkillSelect(int skillId, int skillLevel) 
        {
            CharacterStateContainer characterData = m_characterContext.MemberDataContainer[m_focusedTileId];
            Vector3 pos = characterData.PlayerTransform.position;

            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["SkillId"] = skillId;
            payload["SkillLevel"] = skillLevel;
            CommandEventInfo commandEventInfo = new CommandEventInfo(
                m_focusedTileId, 
                pos, 
                CommandTypes.SKILL_PROMPT, 
                payload);

            OnPlayerCommand?.Invoke(commandEventInfo);
        }

        // Plays OnHide animation
        void HideHUD()
        {

        }

        // Plays OnEnter animation
        void ShowHUD()
        {

        }
    }

}


