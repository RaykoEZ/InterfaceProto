using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor.Tilemaps;
#endif
using BlindChase;

namespace BlindChase
{
    public enum TerrainType 
    { 
        Normal,
        Obstructed,
        HidingSpot
    }

    public class TileProperty : MonoBehaviour
    {
        public TerrainType TerrainType { get; set; }
    }

    [CreateAssetMenu(fileName = "EnvironmentTile", menuName = "Environment Tile", order = 1)]
    public class EnvironmentTile : Tile
    {
        [SerializeField] float m_prefabLocalOffset = 0.5f;
        [SerializeField] float m_prefabZOffset = -1f;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject gameObject)
        {

            if (gameObject != null)
            {
                gameObject.transform.position = new Vector3(position.x + m_prefabLocalOffset
                    , position.y + m_prefabLocalOffset
                    , m_prefabZOffset);

            }

            return true;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
        }

#if UNITY_EDIYOR
        [CreateTileFromPalette]
        public static EnvironmentTile CreateEnvironmentTile(Sprite sprite)
        {
            EnvironmentTile tile = CreateInstance<EnvironmentTile>();
            tile.sprite = sprite;
            tile.name = sprite.name;
            return tile;
        }
#endif
    }
}


