using UnityEngine;
using UnityEngine.Tilemaps;

namespace BlindChase
{
    // For spawning basic tiles on a map.
    public class TileSpawner : MonoBehaviour
    {

        public virtual GameObject SpawnTile(GameObject objectRef, Vector3 position, Transform parent, bool isActive = true) 
        {
            GameObject o = Instantiate(objectRef, position, Quaternion.identity, parent);
            o.SetActive(isActive);
            //Debug.Log($"{Context.WorldMap.size}");
            return o;
        }
    }
}