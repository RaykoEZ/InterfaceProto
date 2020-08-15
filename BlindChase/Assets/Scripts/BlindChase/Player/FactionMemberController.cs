using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.State;


namespace BlindChase
{
    // Used by PlayerCommand to execute different actions in a player's turn
    public class FactionMemberController
    {
        ControllableTileManager m_tileManager;
        FactionContextFactory m_playerContextFactory = default;
        // The id of the faction member tile object we are controlling.
        TileId m_targetId;

        public void Init(
            ControllableTileManager tilemanager, 
            FactionContextFactory p) 
        {
            m_tileManager = tilemanager;
            m_playerContextFactory = p;
            m_tileManager.OnTileSelect += SetControllerTarget;
        }

        // When the player selects a member tile on the map, we start controlling the piece
        void SetControllerTarget(TileId tileId) 
        {
            m_targetId = tileId;
            m_playerContextFactory.FactionMemberSelected(tileId);
        }

        public void MovePlayer(Vector3Int destination, Vector3Int origin)
        {
            Vector3 offset = destination - origin;

            m_tileManager.MoveTile(m_targetId, offset);

            ControllableTileContextData newPlayerData = new ControllableTileContextData(
                destination, 
                m_playerContextFactory.Context.FactionMembers[m_targetId].PlayerTransform);
            
            m_playerContextFactory.UpdateContext(m_targetId, newPlayerData);

        }

    }

}


