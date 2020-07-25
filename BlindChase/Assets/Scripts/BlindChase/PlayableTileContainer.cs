using UnityEngine;
using BlindChase;

namespace BlindChase 
{
    public class PlayableTileContainer : TileContainer
    {
        public TileItem Player { get; private set; }

        public PlayableTileContainer(TileItem item) : base(item)
        {
            Player = item;
        }

        public override void HideTiles()
        {

            Player.TileObject?.SetActive(false);
            Player.Behaviour?.UnlistenToEvents(OnTileEventTrigger);


            isActive = false;
        }

        public override void ShowTiles()
        {

            Player.TileObject?.SetActive(true);
            Player.Behaviour?.ListenToEvents(OnTileEventTrigger);

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

            Player.Behaviour?.UnlistenToEvents(OnTileEventTrigger);
            Object.Destroy(Player.TileObject);

        }

    }

}

