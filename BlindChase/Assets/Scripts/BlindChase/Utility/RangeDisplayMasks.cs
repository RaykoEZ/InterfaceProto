using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Utility 
{
    // A storage for all range patterns for game pieces in the game
    [CreateAssetMenu(fileName = "RangeTileMasks", menuName = "BlindChase/Create cache for range tile masks", order = 1)]
    public class RangeDisplayMasks : ScriptableObject
    {
        [SerializeField] List<RangeMap> m_squareRadiusRangeMasks = new List<RangeMap>();

        public RangeMap GetSquareRadiusMap(int range) 
        {
            if (range < 0 || range > m_squareRadiusRangeMasks.Count) 
            {
                return null;
            }

            return m_squareRadiusRangeMasks[range];

        }

        public void GenerateSquareRadiusMaps(int maxRange) 
        {
            m_squareRadiusRangeMasks.Clear();
            for (int i = 0; i< maxRange + 1; ++i) 
            {
                RangeMap mask = NeighbourhoodUtil.GetNeighbourRangeMap(i);
                m_squareRadiusRangeMasks.Add(mask);
            }
        }
    }
}


