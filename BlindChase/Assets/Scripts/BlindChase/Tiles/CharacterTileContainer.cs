using UnityEngine;
using BlindChase.Events;


namespace BlindChase 
{
    public class CharacterTileContainer : TileContainer
    {
        public CharacterTileItem Player { get; private set; }
        

        public CharacterTileContainer(CharacterTileItem item)
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

        public void SelectCharacter() 
        {
            Player.Behaviour?.OnSelect();
        }

        public void UnselectCharacter() 
        {
            Player.Behaviour?.OnUnselect();
        }

        public void OnCharacterSkillActivate(EventInfo info)
        {
            Player.Behaviour?.OnSkillActivate(info);
        }

        public void OnCharacterAttack(EventInfo info)
        {
            Player.Behaviour?.OnAttack(info);
        }

        public void OnCharacterDefeated(EventInfo info)
        {
            Player.Behaviour?.OnSelfDefeated(info);
        }

        public void OnLeaderDefeated(EventInfo info)
        {
            Player.Behaviour?.OnLeaderDefeated(info);

        }
        public void OnCharacterTakeDamage(EventInfo info)
        {
            Player.Behaviour?.OnTakeDamage(info);
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

