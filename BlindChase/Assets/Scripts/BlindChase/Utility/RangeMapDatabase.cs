using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BlindChase.Utility 
{
    // A storage for all range patterns for game pieces in the game
    [CreateAssetMenu(fileName = "RangeTileMapDatabase", menuName = "BlindChase/Create cache for range tile maps", order = 1)]
    public class RangeMapDatabase : ScriptableObject
    {
        [SerializeField] List<RangeMap> m_squareRadiusRangeMaps = new List<RangeMap>();
        
        Dictionary<string, RangeMap> m_skillRangeMaps;
        Dictionary<CharacterClassType, RangeMap> m_characterClassesRangeMaps;
        
        const string c_classRangeDataRoot = "RangeMaps/ClassRanges";
        const string c_skillRangeDataRoot = "RangeMaps/SkillRanges";

        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter =
        new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;
            TextAsset file = Resources.Load<TextAsset>(c_classRangeDataRoot);
            m_characterClassesRangeMaps = JsonConvert.DeserializeObject<Dictionary<CharacterClassType, RangeMap>>(file.text, m_enumConverter);

            file = Resources.Load<TextAsset>(c_skillRangeDataRoot);
            m_skillRangeMaps = JsonConvert.DeserializeObject<Dictionary<string, RangeMap>>(file.text, m_enumConverter);
        }

        public RangeMap GetClassRangeMap(CharacterClassType type)
        {
            if (type == CharacterClassType.UnKnown || !m_characterClassesRangeMaps.ContainsKey(type)) 
            {
                return null;
            }

            return m_characterClassesRangeMaps[type];
        }

        public RangeMap GetSkillRangeMap(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !m_skillRangeMaps.ContainsKey(id))
            {
                return null;
            }

            return m_skillRangeMaps[id];
        }

        public RangeMap GetSquareRadiusMap(int range) 
        {
            if (range < 0 || range > m_squareRadiusRangeMaps.Count) 
            {
                return null;
            }

            return m_squareRadiusRangeMaps[range];

        }

        public void GenerateSquareRadiusMaps(int maxRange) 
        {
            m_squareRadiusRangeMaps.Clear();
            for (int i = 0; i< maxRange + 1; ++i) 
            {
                RangeMap mask = NeighbourhoodUtil.GetNeighbourRangeMap(i);
                m_squareRadiusRangeMaps.Add(mask);
            }
        }
    }
}


