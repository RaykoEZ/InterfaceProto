using UnityEngine;
using BlindChase.Events;


namespace BlindChase
{
    // Used by PlayerCommand to execute different actions in a player's turn
    public class TileController
    {
        ControllableTileManager m_tileManager;
        GameStateContext m_worldContext = default;
        // The id of the faction member tile object we are controlling.
        TileId m_targetId;
        ControllableTileContextFactory m_characterContextFactoryRef;
        CharacterContext m_characterContext;

        public void Init(
            ControllableTileContextFactory c,
            ControllableTileManager tilemanager, 
            GameStateContextFactory w) 
        {
            m_characterContextFactoryRef = c;
            c.OnContextChanged += OnCharacterContextUpdate;
            m_tileManager = tilemanager;
            m_tileManager.OnTileSelect += SetControllerTarget;
            w.SubscribeToContextUpdate(OnWorldUpdate);
        }


        // When the player selects a member tile on the map, we start controlling the piece
        void SetControllerTarget(TileId tileId) 
        {
            m_targetId = tileId;
        }

        void OnWorldUpdate(GameStateContext world)
        {
            m_worldContext = world;
        }

        void OnCharacterContextUpdate(CharacterContext newContext) 
        {
            m_characterContext = newContext;
        }

        public void MovePlayer(Vector3 destination, Vector3 origin)
        {
            Vector3 offset = destination - origin;

            m_tileManager.MoveTile(m_targetId, offset);

            Transform playerTransform = m_characterContext.MemberDataContainer[m_targetId].PlayerTransform;

            Vector3Int dest = m_worldContext.WorldMap.LocalToCell(destination);
            ControllableDataContainer newPlayerData = new ControllableDataContainer(
                dest,
                playerTransform);

            m_characterContextFactoryRef.UpdateContext(m_targetId, newPlayerData);

        }

    }

}


