using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.State;

namespace BlindChase
{
    public class FactionInfo
    {
        public List<TileId> FactionMemberIds { get; private set; }
        public TileId LeaderId { get { return FactionMemberIds[0]; } private set { LeaderId = value; } }

        public string FactionId { get; private set; }

        public FactionInfo(List<TileId> memeberIds) 
        {
            FactionMemberIds = memeberIds;
            LeaderId = memeberIds[0];
            FactionId = LeaderId.FactionId;
        }
    }

    public class Faction
    {
        PlayerManager m_playerManager = new PlayerManager();


        public FactionInfo m_factionInfo { get; private set; }
        public Faction(
            FactionInfo factionInfo,
            ControllableTileManager tilemanager,
            OptionManager options,
            GameStateManager state,
            FactionMemberController c, WorldContextFactory w,
            FactionContextFactory p) 
        {
            m_factionInfo = factionInfo;
            m_playerManager.Init(tilemanager, options, state, c, w, p);
        }
    }

    public class FactionManager
    {
        Queue<Faction> m_waitingFactions;
        List<Faction> m_finishedFactions = new List<Faction>();

        public FactionManager(List<Faction> factions)
        {
            m_waitingFactions = new Queue<Faction>(factions);
        }
    }

}


