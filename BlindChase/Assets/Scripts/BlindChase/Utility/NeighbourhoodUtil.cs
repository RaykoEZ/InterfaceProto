using System.Collections.Generic;
using UnityEngine;

namespace BlindChase.Utility
{
    [System.Serializable]
    public class NeighbourhoodRangeMap
    {
        public List<Vector3Int> OffsetsFromOrigin;
        public int Range;
    }

    public static class NeighbourhoodUtil 
    {
        /// <summary>
        /// This function gets the offsets needed for defining a range of valid positions to move into, given the movement range. 
        /// </summary>
        /// <param name="range"></param> The max number of squares allowed to move from origin.
        /// <returns></returns> A set of offsets to define valid positions to in a range 
        public static NeighbourhoodRangeMap GetNeighbourRangeMap(int range)
        {
            if (range < 1) 
            {
                return new NeighbourhoodRangeMap 
                { 
                    OffsetsFromOrigin = new List<Vector3Int>{Vector3Int.zero}, 
                    Range = 0 
                };
            }

            // Getting the immediate neighbours is quicker here
            if (range == 1) 
            {
                return GetImmediateNeighbourhood();
            }

            /// We process each positive coords within range, then we reflect them in each x-y region to create a square range. 
            /// n-squared complexity but called once for each different ranges.
            /// 
            /// 2
            /// 1 1
            /// 0 0 0
            List<Vector3Int> neighbours = new List<Vector3Int>();

            for (int i = 0; i <= range; ++i ) 
            {
                for (int j = range - i; j >= 0; --j) 
                {
                    if (i == 0 && j == 0) 
                    {
                        break;
                    }

                    neighbours.Add(new Vector3Int (j, i, 0));
                    neighbours.Add(new Vector3Int (-j, i, 0));
                    neighbours.Add(new Vector3Int (j, -i, 0));
                    neighbours.Add(new Vector3Int (-j, -i, 0));
                }
            }

            NeighbourhoodRangeMap map = new NeighbourhoodRangeMap { OffsetsFromOrigin = neighbours, Range = range};

            return map;
        }

        static NeighbourhoodRangeMap GetImmediateNeighbourhood() 
        {
            List<Vector3Int> neighbours = new List<Vector3Int>();
            neighbours.Add(new Vector3Int(0,1, 0));
            neighbours.Add(new Vector3Int(0, -1, 0));
            neighbours.Add(new Vector3Int(1, 0, 0));
            neighbours.Add(new Vector3Int(-1, 0, 0));

            NeighbourhoodRangeMap map = new NeighbourhoodRangeMap 
            { 
                OffsetsFromOrigin = neighbours, 
                Range = 1 
            };

            return map;
        }

    }

}


