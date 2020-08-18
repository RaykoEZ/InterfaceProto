using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{
    // This class encapsulates all data needed to describe a faction of player pieces.
    public class FactionDataContainer
    {
        public string FactionId { get; private set; }
        public FactionContext FactionMembers { get; private set; }
        
        public FactionDataContainer(FactionContextFactory contextFactory)
        {
            contextFactory.SubscribeToContextUpdate(OnMemberUpdate);
            FactionMembers = contextFactory.Context;
            FactionId = contextFactory.FactionId;
        }

        void OnMemberUpdate(FactionContext newContext)
        {
            FactionMembers = newContext;
        }
    }

    public class FactionManager
    {
        List<FactionDataContainer> m_factions { get; set; }
        Dictionary<string, FactionContextFactory> m_contextFactoryRefs;

        public FactionDataContainer FactionContainer(string factionId)
        {
            foreach (FactionDataContainer data in m_factions)
            {
                if (data.FactionId == factionId)
                {
                    return data;
                }
            }

            Debug.LogError($"No faction with Id: {factionId} was found.");
            return null;
        }

        public void UpdateFactionData(TileId targetId, ControllableDataContainer newContextData) 
        {
            m_contextFactoryRefs[targetId.FactionId].UpdateContext(targetId, newContextData);
        }

        public FactionManager(List<FactionContextFactory> contextFactories)
        {
            m_contextFactoryRefs = new Dictionary<string, FactionContextFactory>();

            m_factions = new List<FactionDataContainer>();

            foreach (FactionContextFactory factories in contextFactories)
            {
                m_factions.Add(new FactionDataContainer(factories));

                m_contextFactoryRefs[factories.FactionId] = factories;
            }
        }
    }

}


