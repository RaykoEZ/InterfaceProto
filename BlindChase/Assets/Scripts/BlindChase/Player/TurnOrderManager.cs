using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
using BlindChase.State;

namespace BlindChase
{
    public class TurnOrderManager
    {
        Queue<FactionDataContainer> m_waitingFactions;
        List<FactionDataContainer> m_finishedFactions = new List<FactionDataContainer>();

        public TurnOrderManager(List<FactionDataContainer> factions)
        {
            m_waitingFactions = new Queue<FactionDataContainer>(factions);
        }

        public FactionDataContainer GetActiveFaction() 
        {
            return m_waitingFactions.Peek();
        }

        public void TurnEnd()
        {
            m_finishedFactions.Add(m_waitingFactions.Dequeue());

            if (m_waitingFactions.Count == 0)
            {
                m_waitingFactions = new Queue<FactionDataContainer>(m_finishedFactions);
            }
        }

    }

}


