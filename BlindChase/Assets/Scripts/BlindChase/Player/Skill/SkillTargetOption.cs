using System;
using System.Collections.Generic;
using UnityEngine;
using BlindChase.Utility;

namespace BlindChase
{
    // When skill target is confirmed, invoke this.
    public delegate void OnSkillTargetChosen();
    // Effects will prompt player to choose there target in range
    public class SkillTargetOption
    {
        public RangeMap TargetRangePattern { get; private set; }

        public event OnSkillTargetChosen OnTargetChosen = default;

        public SkillTargetOption(RangeMap rangeMap) 
        {
            TargetRangePattern = rangeMap;
        }
    }

}


