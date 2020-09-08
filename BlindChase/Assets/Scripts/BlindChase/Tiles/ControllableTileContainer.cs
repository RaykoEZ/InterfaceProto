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

        public void AllowPlayerCommand()
        {
            Player.Behaviour?.ListenToCommands(OnTileCommandTrigger);
            Player.Behaviour?.OnStartCommand();
        }

        public void DisallowPlayerCommand() 
        {
            Player.Behaviour?.UnlistenToCommands(OnTileCommandTrigger);
            Player.Behaviour?.OnFinishCommand();
        }

        public void SelectTile() 
        {
            Player.Behaviour?.OnPlayerPointerSelect();
        }

        public void UnselectTile() 
        {
            Player.Behaviour?.OnPlayerPointerSelect();
        }

        public override void HideTiles()
        {
            Player.TileObject?.SetActive(false);
            Player.Behaviour?.UnlistenToSelection(OnTileSelected);

            isActive = false;
        }

        public override void ShowTiles()
        {
            Player.TileObject?.SetActive(true);
            Player.Behaviour?.ListenToSelection(OnTileSelected);

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

            DisallowPlayerCommand();
            Player.Behaviour?.Shutdown();
            Player.Behaviour?.UnlistenToSelection(OnTileSelected);
            Object.Destroy(Player.TileObject);

        }

    }

}

