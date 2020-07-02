using UnityEngine;
using System.Collections.Generic;

namespace BlindChase 
{
    // Stores tile gameobjects generated in a session.
    public class TileContainer
    {
        public List<GameObject> Tiles { get; private set; } = new List<GameObject>();
        public bool isActive { get; private set; } = false; 

        public void HideTiles() 
        {
            foreach (GameObject o in Tiles) 
            {
                o?.SetActive(false);
            }
            isActive = false;
        }

        public void ShowTiles() 
        {
            foreach (GameObject o in Tiles)
            {
                o?.SetActive(true);
            }
            isActive = true;
        }

        public void MoveTiles(Vector3 diff) 
        {
            foreach (GameObject o in Tiles)
            {
                o.transform.position += diff;
            }
            isActive = true;
        }
    }
}

