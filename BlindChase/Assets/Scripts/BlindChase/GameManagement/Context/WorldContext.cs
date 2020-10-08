using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace BlindChase.GameManagement
{
    [SerializeField]
    public class BoardState 
    { 
        public Dictionary<Vector3Int, ObjectId> BoardOccupiers { get; set; }

        public BoardState() 
        {
            BoardOccupiers = new Dictionary<Vector3Int, ObjectId>();
        }
    }

    public class WorldContext : IBCContext
    {
        public Tilemap WorldMap { get; private set; }

        BoardState m_boardState { get; set; }

        public ObjectId GetOccupyingTileAt(Vector3Int pos)
        {
            if (!m_boardState.BoardOccupiers.ContainsKey(pos)) 
            {
                return null;
            }

            return m_boardState.BoardOccupiers[pos];
        }

        public WorldContext(Tilemap map)
        {
            WorldMap = map;
            m_boardState = new BoardState();
        }

        public WorldContext(WorldContext w) 
        {
            WorldMap = w.WorldMap;
            if(m_boardState == null) 
            {
                m_boardState = w.m_boardState;
                return;
            }

            foreach(Vector3Int pos in w.m_boardState.BoardOccupiers.Keys) 
            {
                m_boardState.BoardOccupiers[pos] = w.m_boardState.BoardOccupiers[pos];
            }

        }

        public void UpdateBoardState(Vector3Int pos, ObjectId id) 
        {
            m_boardState.BoardOccupiers[pos] = id;
        }

        public void RemoveBoardPiece(Vector3Int pos, ObjectId id) 
        {
            if (m_boardState.BoardOccupiers.ContainsKey(pos) && m_boardState.BoardOccupiers[pos] == id) 
            {
                m_boardState.BoardOccupiers.Remove(pos);
            }
        }
    }


}

