using System.Collections.Generic;
using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public interface ITileManager
    {

        event OnTileTrigger<TileEventInfo> OnTileEvent;

        void Init();
        void Shutdown();
        GameObject SpawnTile(string id, GameObject objectRef, Vector3 position, Transform parent, bool isActive = true);

        void DespawnTiles(string id);
        void MoveTile(string id, Vector3 offset);
    }
}

