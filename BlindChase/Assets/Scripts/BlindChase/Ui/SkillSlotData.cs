using UnityEngine;
using BlindChase.GameManagement;

namespace BlindChase.UI
{
    public struct SkillSlotData
    {
        public int SkillId;
        public int SkillLevel;
        public int Cooldown;
        public Sprite SkillIcon;
        public SkillDataCollection SkillData;

        public SkillSlotData(int id, int level, int cooldown, Sprite icon, SkillDataCollection dataCollection)
        {
            SkillId = id;
            SkillLevel = level;
            Cooldown = cooldown;
            SkillIcon = icon;
            SkillData = dataCollection;
        }
    }

}


