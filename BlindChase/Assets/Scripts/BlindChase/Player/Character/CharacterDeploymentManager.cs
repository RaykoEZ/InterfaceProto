using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

namespace BlindChase
{
    [Serializable]
    public struct FactionDeploymentInfo
    {
        public string FactionId;
        public List<UnitDeploymentInfo> SpawnList;
    }

    [Serializable]
    public struct UnitDeploymentInfo 
    {
        public string CharacterId;
        public Vector3Int SpawnLocation;
    }

    [Serializable]
    public struct SpawnCoordinates 
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }
    [Serializable]
    public struct CharacterDeploymentList
    {
        public List<FactionDeploymentInfo> DeploymentInfo;
    } 

    // - Taking JSON inputs for enemy deployment data
    // - Take Input Data from player.
    public class CharacterDeploymentManager
    {
        TextAsset m_textAsset;
        const string c_deploymentResourcePrefix = "UnitDeployment/Deploy";
        CharacterDatabase m_characterDatabase;

        public void Init(string mapKey)
        {
            m_textAsset = Resources.Load<TextAsset>($"{c_deploymentResourcePrefix}{mapKey}");
            m_characterDatabase = ScriptableObject.CreateInstance<CharacterDatabase>();
        }

        public CharacterDeploymentList GetNPCDeployment()
        {
            CharacterDeploymentList factionDeployment;
            factionDeployment = JsonConvert.DeserializeObject<CharacterDeploymentList>(m_textAsset.text);

            return factionDeployment;
        }

        public List<CharacterState> DeployUnits(
            Transform parent,
            List<FactionDeploymentInfo> factionDeployment,
            GameObject objectRef,
            Tilemap tilemap,
            ControllableTileManager tileManager,
            CharacterContextFactory characterContextFactory
            ) 
        {
            Dictionary<TileId, ControllableDataContainer> contextData = new Dictionary<TileId, ControllableDataContainer>();
            List<CharacterState> ret = new List<CharacterState>();

            foreach (FactionDeploymentInfo info in factionDeployment) 
            {
                // For each unit in this faction, spawn them accordingly.
                for (int i = 0; i < info.SpawnList.Count; ++i)
                {
                    TileId id = new TileId(CommandTypes.NONE, info.FactionId, i.ToString());

                    UnitDeploymentInfo unitDeploymentInfo = info.SpawnList[i];

                    Vector3Int spawnCoord = unitDeploymentInfo.SpawnLocation;

                   //Vector3Int coord = new Vector3Int(spawnCoord.x, spawnCoord.y, spawnCoord.z);

                    Vector3 p = tilemap.GetCellCenterLocal(spawnCoord);

                    GameObject playerObject = tileManager.SpawnTile(
                        id,
                        objectRef,
                        p,
                        parent
                    );

                    CharacterData charData = m_characterDatabase.GetCharacterData(unitDeploymentInfo.CharacterId);

                    CharacterState characterState = new CharacterState(
                        id,
                        unitDeploymentInfo.CharacterId,
                        charData,
                        spawnCoord);
                    ret.Add(characterState);

                    ControllableDataContainer playerData = new ControllableDataContainer(playerObject.transform, characterState);

                    tileManager.ShowTile(id);
                    contextData[id] = playerData;

                }
            }

            CharacterContext newContext = new CharacterContext(contextData);
            characterContextFactory.Update(newContext);
            return ret;
        }
    }

}


