using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BlindChase
{
    // For spawning basic tiles on a map.
    public class TileSpawner
    {
        public virtual GameObject SpawnTile(
            TileId tileId, GameObject objectRef, Vector3 position, Transform parent,
            CharacterData charData = default, RangeDisplay rangeDisplayRef = null, bool isActive = true
            ) 
        {
            GameObject o = Object.Instantiate(objectRef, position, Quaternion.identity, parent);
            TileBehaviour behaviour = o.GetComponent<TileBehaviour>();
            
            if (behaviour != null) 
            {
                behaviour?.Init(tileId, rangeDisplayRef, charData);
            }

            o.SetActive(isActive);
            return o;
        }
    }
}