using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase;

namespace BlindChase
{
    public struct FactionDeploymentInfo 
    {
        public FactionContext UnitInfo;
        
        public FactionDeploymentInfo(FactionContext units) 
        {
            UnitInfo = units;
        }
    }

}


