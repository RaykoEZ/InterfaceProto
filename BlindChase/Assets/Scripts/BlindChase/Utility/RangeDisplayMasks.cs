using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Utility 
{
    // A storage for all range patterns for game pieces in the game
    [CreateAssetMenu(fileName = "RangeTileMasks", menuName = "BlindChase/Create cache for range tile masks", order = 1)]
    public class RangeDisplayMasks : ScriptableObject
    {
        [SerializeField] List<NeighbourhoodRangeMap> SquareRadiusRangeMasks = new List<NeighbourhoodRangeMap>();
        public void GenerateSquareRadiusMaps(int maxRange) 
        {
            SquareRadiusRangeMasks.Clear();
            for (int i = 0; i< maxRange + 1; ++i) 
            {
                NeighbourhoodRangeMap mask = NeighbourhoodUtil.GetNeighbourRangeMap(i);
                SquareRadiusRangeMasks.Add(mask);
            }
        }
    }
}


