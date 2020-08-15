using UnityEngine;
using BlindChase;

namespace BlindChase 
{
    public class ControllableTileContainer : TileContainer
    {
        public TileItem Player { get; private set; }

        public ControllableTileContainer(TileItem item) : base(item)
        {
            Player = item;
        }

        public override void HideTiles()
        {
            Player.TileObject?.SetActive(false);
            Player.Behaviour?.UnlistenToEvents(OnTileEventTrigger);
            Player.Behaviour?.UnlistenToTileSelection(OnTileEventSelected);


            isActive = false;
        }

        public override void ShowTiles()
        {

            Player.TileObject?.SetActive(true);
            Player.Behaviour?.ListenToEvents(OnTileEventTrigger);
            Player.Behaviour?.ListenToTileSelection(OnTileEventSelected);

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
            Player.Behaviour?.UnlistenToTileSelection(OnTileEventSelected);
            Object.Destroy(Player.TileObject);

        }

    }

}

