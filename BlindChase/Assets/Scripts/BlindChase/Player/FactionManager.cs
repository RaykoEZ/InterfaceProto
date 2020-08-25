using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.State;


namespace BlindChase
{
    /*
    // This class encapsulates all data needed to describe a faction of player pieces.
    public class FactionDataContainer
    {
        public string FactionId { get; private set; }
        public CharacterContext FactionMembers { get; private set; }
        
        public FactionDataContainer(CharacterContextFactory contextFactory)
        {
            contextFactory.SubscribeToContextUpdate(OnMemberUpdate);
            FactionMembers = contextFactory.Context;
        }

        void OnMemberUpdate(CharacterContext newContext)
        {
            FactionMembers = newContext;
        }
    }

    public class FactionManager
    {
        List<FactionDataContainer> m_factions { get; set; }
        Dictionary<string, CharacterContextFactory> m_contextFactoryRefs;

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

        public List<FactionDataContainer> GetAllFactionData() 
        {
            return m_factions;
        }

        public void UpdateFactionData(TileId targetId, ControllableDataContainer newContextData) 
        {
            m_contextFactoryRefs[targetId.FactionId].UpdateContext(targetId, newContextData);
        }

        public FactionManager(List<CharacterContextFactory> contextFactories)
        {
            m_contextFactoryRefs = new Dictionary<string, CharacterContextFactory>();

            m_factions = new List<FactionDataContainer>();

            foreach (CharacterContextFactory factories in contextFactories)
            {
                m_factions.Add(new FactionDataContainer(factories));

                //m_contextFactoryRefs[factories.FactionId] = factories;
            }
        }
    }
    */
}


