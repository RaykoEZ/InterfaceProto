using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BlindChase.Utility;

namespace BlindChase.GameManagement 
{
    // A storage for all range patterns for game pieces in the game
    [CreateAssetMenu(fileName = "RangeTileMapDatabase", menuName = "BlindChase/Create cache for range tile maps", order = 1)]
    public class RangeMapDatabase : ScriptableObject
    {
        // Internal data structure to deserialize json string into, need to convert to external type for general use.
        [Serializable]
        private class RangeOffsetMap_Internal
        {
            public List<Vector3Int> OffsetsFromOrigin = default;

            [JsonConstructor]
            public RangeOffsetMap_Internal(List<Vector3Int> offsets)
            {
                OffsetsFromOrigin = offsets;
            }

            public RangeOffsetMap_Internal(RangeOffsetMap map) 
            {
                OffsetsFromOrigin = new List<Vector3Int>(map.OffsetsFromOrigin);
            }

            public RangeOffsetMap ToExternal() 
            {
                return new RangeOffsetMap(OffsetsFromOrigin);
            } 
        }

        [SerializeField] List<RangeOffsetMap_Internal> m_squareRadiusRangeMaps = new List<RangeOffsetMap_Internal>();
        
        Dictionary<string, RangeOffsetMap> m_skillRangeMaps;
        Dictionary<CharacterClassType, RangeOffsetMap> m_characterClassesRangeMaps;
        
        const string c_classRangeDataRoot = "RangeMaps/ClassRanges";
        const string c_skillRangeDataRoot = "RangeMaps/SkillRanges";

        static Newtonsoft.Json.Converters.StringEnumConverter m_enumConverter =
        new Newtonsoft.Json.Converters.StringEnumConverter();

        void OnEnable()
        {
            m_enumConverter.AllowIntegerValues = true;
            TextAsset file = Resources.Load<TextAsset>(c_classRangeDataRoot);

            Dictionary<CharacterClassType, RangeOffsetMap_Internal> internalMoveRange = JsonConvert.DeserializeObject<Dictionary<CharacterClassType, RangeOffsetMap_Internal>>(file.text, m_enumConverter);
            m_characterClassesRangeMaps = ToExternal(internalMoveRange);

            file = Resources.Load<TextAsset>(c_skillRangeDataRoot);
            Dictionary<string, RangeOffsetMap_Internal> internalSkillRange = JsonConvert.DeserializeObject<Dictionary<string, RangeOffsetMap_Internal>>(file.text, m_enumConverter);
            m_skillRangeMaps = ToExternal(internalSkillRange);
        }

        Dictionary<T, RangeOffsetMap> ToExternal<T>(Dictionary<T, RangeOffsetMap_Internal> internalCollection) 
        {
            Dictionary<T, RangeOffsetMap> ret = new Dictionary<T, RangeOffsetMap>();
            foreach (KeyValuePair<T, RangeOffsetMap_Internal> kvp in internalCollection)
            {
                ret[kvp.Key] = kvp.Value.ToExternal();
            }
            return ret;
        }

        public RangeOffsetMap GetAttackRangeMap(CharacterClassType type)
        {
            if (type == CharacterClassType.UnKnown || !m_characterClassesRangeMaps.ContainsKey(type)) 
            {
                return null;
            }

            return m_characterClassesRangeMaps[type];
        }

        public RangeOffsetMap GetSkillRangeMap(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !m_skillRangeMaps.ContainsKey(id))
            {
                return null;
            }

            return m_skillRangeMaps[id];
        }

        public RangeOffsetMap GetSquareRadiusMap(int range) 
        {
            if (range < 0 || range > m_squareRadiusRangeMaps.Count) 
            {
                return null;
            }

            return m_squareRadiusRangeMaps[range].ToExternal();

        }

        public void GenerateSquareRadiusMaps(int maxRange) 
        {
            m_squareRadiusRangeMaps.Clear();
            for (int i = 0; i< maxRange + 1; ++i) 
            {
                RangeOffsetMap_Internal mask = new RangeOffsetMap_Internal(NeighbourhoodUtil.GetNeighbourRangeMap(i));
                m_squareRadiusRangeMaps.Add(mask);
            }
        }
    }
}


