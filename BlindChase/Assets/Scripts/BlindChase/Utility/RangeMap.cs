using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase.Utility
{
    [Serializable]
    public class RangeMap
    {
        [NonSerialized]
        IReadOnlyList<Vector3Int> m_offsets;

        //List for common operations
        List<Vector3Int> m_offsetList;
        public IReadOnlyList<Vector3Int> OffsetsFromOrigin { get { return m_offsets; }}
        public RangeMap(List<Vector3Int> rangeTiles) 
        {
            m_offsets = rangeTiles;
            m_offsetList = rangeTiles;
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


