using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.GameManagement;

namespace BlindChase.Ai
{
    public class NpcManager
    {
        public event OnPlayerCommand<CommandStackInfo> OnNPCCommand = default;

        NpcDatabase m_npcDatabase = default;
        // One planner for each faction, key: Faction Id
        //HashSet<string> m_npcFactions = new HashSet<string>();
        NpcTaskPlanner m_planner = new NpcTaskPlanner();

        GameContextCollection m_context = new GameContextCollection();

        public void Init(CharacterContextFactory c , WorldStateContextFactory w, SkillManager skillManager) 
        {
            m_npcDatabase = ScriptableObject.CreateInstance<NpcDatabase>();

            c.OnContextChanged += UpdateCharacter;
            w.OnContextChanged += UpdateWorld;
            m_planner.OnNPCTaskPlanned += OnNpcAction;
            m_planner.Init(skillManager);

            UpdateCharacter(c.Context);
            UpdateWorld(w.Context);
        }

        public void Shutdown() 
        {
            OnNPCCommand = null;
            m_planner.Shutdown();
        }

        public void OnNPCActive(ObjectId id) 
        {
            string faction = id.FactionId;
            NPCDetail factionNature = m_npcDatabase.GetNpcNature(faction);

            if(factionNature == null) 
            {
                Debug.LogError("NpcMananger - Cannot find correct Npc detail.");
                return;
            }

            m_planner.OnActive(id, factionNature, m_context);
        }
        void OnNpcAction(CommandStackInfo info)
        {
            OnNPCCommand?.Invoke(info);
        }

        void UpdateCharacter(CharacterContext character) 
        {
            m_context.Characters = character;
        }

        void UpdateWorld(WorldContext world)
        {
            m_context.World = world;
        }
    }

}


