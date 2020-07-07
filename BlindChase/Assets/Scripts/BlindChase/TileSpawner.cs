using UnityEngine;
using UnityEngine.Tilemaps;
using BlindChase.Events;

namespace BlindChase
{
    // For spawning basic tiles on a map.
    public class TileSpawner : MonoBehaviour
    {

        public virtual GameObject SpawnTile(GameObject objectRef, Vector3 position, Transform parent, BCEventHandler e = null, bool isActive = true) 
        {
            GameObject o = Instantiate(objectRef, position, Quaternion.identity, parent);
            TileBehaviour behaviour = o.GetComponent<TileBehaviour>();
            
            if (behaviour != null) 
            {
                behaviour?.Init(e);
            }

            o.SetActive(isActive);
            return o;
        }
    }
}