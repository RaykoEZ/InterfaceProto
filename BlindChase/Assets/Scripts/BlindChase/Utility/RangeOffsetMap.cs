using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase.Utility
{
    public class RangeOffsetMap
    {
        //List for common operations
        List<Vector3Int> m_offsetList;
        public IReadOnlyList<Vector3Int> OffsetsFromOrigin { get { return m_offsetList; }}
        public RangeOffsetMap(IEnumerable<Vector3Int> rangeTiles) 
        {
            m_offsetList = new List<Vector3Int>(rangeTiles);
        }

        public RangeOffsetMap(List<Vector3Int> rangeTiles)
        {
            m_offsetList = rangeTiles;
        }

        public List<Vector3Int> ApplyRangeOffsets(Vector3Int origin) 
        {
            List<Vector3Int> ret = new List<Vector3Int>();
            foreach (Vector3Int offset in m_offsetList) 
            {
                ret.Add(origin + offset);
            }

            return ret;
        }

        public bool IsInRange(Vector3Int origin, Vector3Int target) 
        {
            return m_offsetList.Contains(target - origin);
        }

        public bool IsInRange(Vector3Int offset)
        {
            return m_offsetList.Contains(offset);
        }
    }
}


