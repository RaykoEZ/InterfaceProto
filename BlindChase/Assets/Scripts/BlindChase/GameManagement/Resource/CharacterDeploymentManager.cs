﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using BlindChase.UI;

namespace BlindChase.GameManagement
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
        public string NpcId;
        public Vector3Int SpawnLocation;
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
            CharacterManager tileManager,
            WorldStateContextFactory worldStateContextFactory,
            CharacterContextFactory characterContextFactory,
            PromptHandler rangeDisplay
            ) 
        {
            Dictionary<ObjectId, CharacterStateContainer> contextData = new Dictionary<ObjectId, CharacterStateContainer>();
            List<CharacterState> ret = new List<CharacterState>();
            WorldContext worldContext = new WorldContext(tilemap);
            foreach (FactionDeploymentInfo info in factionDeployment) 
            {
                // For each unit in this faction, spawn them accordingly.
                for (int i = 0; i < info.SpawnList.Count; ++i)
                {

                    UnitDeploymentInfo unitDeploymentInfo = info.SpawnList[i];

                    Vector3Int spawnCoord = unitDeploymentInfo.SpawnLocation;

                    Vector3 p = tilemap.GetCellCenterLocal(spawnCoord);

                    ObjectId id = new ObjectId(
                        info.FactionId, 
                        i.ToString(), 
                        typeId: CommandTypes.NONE, 
                        npcId: unitDeploymentInfo.NpcId);

                    worldContext.UpdateBoardState(spawnCoord, id);

                    CharacterData charData = m_characterDatabase.GetCharacterData(unitDeploymentInfo.CharacterId);

                    GameObject playerObject = tileManager.SpawnTile(
                        id,
                        objectRef,
                        p,
                        parent, 
                        charData,
                        rangeDisplay
                    );

                    CharacterState characterState = new CharacterState(
                        id,
                        unitDeploymentInfo.CharacterId,
                        charData,
                        spawnCoord);
                    ret.Add(characterState);

                    CharacterStateContainer playerData = new CharacterStateContainer(playerObject.transform, characterState);

                    tileManager.ShowTile(id);
                    contextData[id] = playerData;

                }
            }

            CharacterContext newContext = new CharacterContext(contextData);
            characterContextFactory.Update(newContext);
            worldStateContextFactory.Update(worldContext);
            return ret;
        }
    }

}


