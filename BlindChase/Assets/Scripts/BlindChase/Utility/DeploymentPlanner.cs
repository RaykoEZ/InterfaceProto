using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;
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
    public struct FactionDeployment 
    {
        public List<DeploymentInfo> DeploymentInfo;
    } 

    // - Taking JSON inputs for enemy deployment data
    // - Take Input Data from player.
    public class DeploymentPlanner : MonoBehaviour
    {
        void Start()
        {
            
        }

        void GetNPCDeployment() 
        { 
        }

        public void ConfirmPlayerDeployment() 
        { 
        
        }
    }

}


