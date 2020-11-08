using System;

namespace BlindChase.Ai
{
    [Serializable]
    public class ObjectiveBias 
    {
        float m_weight;
        public float Weight { get { return m_weight; } set { m_weight = Math.Min(1.0f, Math.Abs(value)); }}

        public ObjectiveBias(float weight) 
        {
            Weight = weight;
        }
    }

}


