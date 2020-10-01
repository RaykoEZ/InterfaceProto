using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public struct CharacterItem
    {
        public GameObject TileObject;
        public CharacterBehaviour Behaviour;
    }

    public class CharacterContainer : TileContainer
    {
        public CharacterItem Player { get; private set; }       

        public CharacterContainer(CharacterItem item)
        {
            Player = item;
        }

        public override void HideTiles()
        {
            Player.TileObject?.SetActive(false);
            isActive = false;
        }

        public override void ShowTiles()
        {
            Player.TileObject?.SetActive(true);
            isActive = true;
        }

        public override void MoveTiles(Vector3 diff)
        {
            Player.TileObject.transform.position += diff;
            isActive = true;
        }

        public override void DestroyTiles()
        {
            isActive = false;
            Player.Behaviour?.Shutdown();
            Object.Destroy(Player.TileObject);
        }
    }

}

