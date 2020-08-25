using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

namespace BlindChase
{
    [Serializable]
    public struct DeploymentInfo
    {
        public string FactionId;
        public List<SpawnCoordinates> SpawnCoordinates;
    }
    [Serializable]
    public struct SpawnCoordinates 
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }
    [Serializable]
    public struct UnitDeploymentList
    {
        public List<DeploymentInfo> DeploymentInfo;
    } 

    // - Taking JSON inputs for enemy deployment data
    // - Take Input Data from player.
    public class DeploymentManager
    {
        TextAsset m_textAsset;
        const string c_deploymentResourcePrefix = "UnitDeployment/Deploy";

        public void Init(string mapKey)
        {
            m_textAsset = Resources.Load<TextAsset>($"{c_deploymentResourcePrefix}{mapKey}");
        }

        public UnitDeploymentList GetNPCDeployment()
        {
            UnitDeploymentList factionDeployment;
            factionDeployment = JsonConvert.DeserializeObject<UnitDeploymentList>(m_textAsset.text);

            return factionDeployment;
        }

        public void DeployUnits(
            Transform parent,
            List<DeploymentInfo> factionDeployment,
            GameObject objectRef,
            Tilemap tilemap,
            ControllableTileManager tileManager,
            ControllableTileContextFactory characterContextFactory
            ) 
        {
            Dictionary<TileId, ControllableDataContainer> contextData = new Dictionary<TileId, ControllableDataContainer>();

            foreach (DeploymentInfo info in factionDeployment) 
            {
                // For each unit in this faction, spawn them accordingly.
                for (int i = 0; i < info.SpawnCoordinates.Count; ++i)
                {
                    TileId id = new TileId(CommandTypes.NONE, info.FactionId, i.ToString());

                    SpawnCoordinates spawnCoord = info.SpawnCoordinates[i];
                    Vector3Int coord = new Vector3Int(spawnCoord.x, spawnCoord.y, spawnCoord.z);

                    Vector3 p = tilemap.GetCellCenterLocal(coord);

                    GameObject playerObject = tileManager.SpawnTile(
                        id,
                        objectRef,
                        p,
                        parent
                    );

                    ControllableDataContainer playerData = new ControllableDataContainer(coord, playerObject.transform);

                    tileManager.ShowTile(id);
                    contextData[id] = playerData;

                }
            }

            CharacterContext newContext = new CharacterContext(contextData);
            characterContextFactory.Update(newContext);
        }
    }

}


