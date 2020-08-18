using UnityEngine;
using BlindChase.Events;


namespace BlindChase
{
    // Used by PlayerCommand to execute different actions in a player's turn
    public class FactionMemberController
    {
        ControllableTileManager m_tileManager;
        WorldContext m_worldContext = default;
        // The id of the faction member tile object we are controlling.
        TileId m_targetId;
        FactionManager m_factionManagerRef;

        public void Init(
            FactionManager factionManager,
            ControllableTileManager tilemanager, 
            WorldContextFactory w) 
        {
            m_factionManagerRef = factionManager;
            m_tileManager = tilemanager;
            m_tileManager.OnTileSelect += SetControllerTarget;
            w.SubscribeToContextUpdate(OnWorldUpdate);
        }


        // When the player selects a member tile on the map, we start controlling the piece
        void SetControllerTarget(TileId tileId) 
        {
            m_targetId = tileId;
        }

        void OnWorldUpdate(WorldContext world)
        {
            m_worldContext = world;
        }

        public void MovePlayer(Vector3 destination, Vector3 origin)
        {
            Vector3 offset = destination - origin;

            m_tileManager.MoveTile(m_targetId, offset);

            Transform playerTransform = m_factionManagerRef.FactionContainer(m_targetId.FactionId).
                FactionMembers.MemberDataContainer[m_targetId].PlayerTransform;

            Vector3Int dest = m_worldContext.WorldMap.LocalToCell(destination);
            ControllableDataContainer newPlayerData = new ControllableDataContainer(
                dest,
                playerTransform);

            m_factionManagerRef.UpdateFactionData(m_targetId, newPlayerData);

        }

    }

}


