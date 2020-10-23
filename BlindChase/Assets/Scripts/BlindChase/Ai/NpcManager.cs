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

        public void Init(RangeMapDatabase rangeDatabase) 
        {
            m_npcDatabase = ScriptableObject.CreateInstance<NpcDatabase>();

            m_planner.OnNPCTaskPlanned += OnNpcAction;
            m_planner.Init(rangeDatabase);
        }

        public void Shutdown() 
        {
            OnNPCCommand = null;
            m_planner.Shutdown();
        }

        public void OnNPCActive(ObjectId id, GameContextRecord context) 
        {
            string faction = id.FactionId;
            DecisionParameter factionNature = m_npcDatabase.GetNpcNature(faction);

            if(factionNature == null) 
            {
                Debug.LogError("NpcMananger - Cannot find correct Npc detail.");
                return;
            }

            m_planner.OnActive(id, factionNature, context);
        }
        void OnNpcAction(CommandRequestInfo info)
        {
            OnNPCCommand?.Invoke(info);
        }
    }

}


