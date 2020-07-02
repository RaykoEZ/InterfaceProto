using UnityEngine;
using UnityEngine.Tilemaps;

namespace BlindChase
{
    // For spawning any tile that has any activities/behaviour e.g. A player, turret etc
    public class SentientTileSpawner : TileSpawner
    {
        public override GameObject SpawnTile(GameObject objectRef, Vector3 position, Transform parent, bool setActive = true)
        {
            GameObject o = base.SpawnTile(objectRef, position, parent);
            // Pass context to potential spawners
            return o;
        }
    }
}