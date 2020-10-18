using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;
using BlindChase.GameManagement;

namespace BlindChase.Ai
{
    public class NpcManager
    {
        public event OnPlayerCommand<CommandRequestInfo> OnNPCCommand = default;

        NpcDatabase m_npcDatabase = default;
        // One planner for each faction, key: Faction Id
        //HashSet<string> m_npcFactions = new HashSet<string>();
        NpcTaskPlanner m_planner = new NpcTaskPlanner();

        CharacterContext m_characterRef;
        WorldContext m_worldRef;

        public void Init(CharacterContextFactory c , WorldStateContextFactory w, RangeMapDatabase rangeDatabase) 
        {
            m_npcDatabase = ScriptableObject.CreateInstance<NpcDatabase>();

            c.OnContextChanged += UpdateCharacter;
            w.OnContextChanged += UpdateWorld;
            m_planner.OnNPCTaskPlanned += OnNpcAction;
            m_planner.Init(rangeDatabase);

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
            NpcParameter factionNature = m_npcDatabase.GetNpcNature(faction);

            if(factionNature == null) 
            {
                Debug.LogError("NpcMananger - Cannot find correct Npc detail.");
                return;
            }

            m_planner.OnActive(id, factionNature, m_characterRef, m_worldRef);
        }
        void OnNpcAction(CommandRequestInfo info)
        {
            OnNPCCommand?.Invoke(info);
        }

        void UpdateCharacter(CharacterContext character) 
        {
            m_characterRef = character;
        }

        void UpdateWorld(WorldContext world)
        {
            m_worldRef = world;
        }
    }

}


