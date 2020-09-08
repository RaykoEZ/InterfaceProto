using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{
    public static class HashingUtility
    {
        public static int Hash2D(int xDim, int x, int y) 
        {
            return xDim * y + x;
        }

        public static Vector3Int GetXYFromIndex(int xDim, int index)
        {
            int y = Mathf.FloorToInt(index/xDim);
            int x = index % xDim;
            return new Vector3Int(x, y, 0);
        }
    }

}


