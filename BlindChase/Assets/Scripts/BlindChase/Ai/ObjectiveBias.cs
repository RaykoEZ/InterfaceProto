using System;

namespace BlindChase.Ai
{
    [Serializable]
    public class ObjectiveBias 
    {
        float m_weight;
        public float Weight { get { return m_weight; } set { m_weight = Math.Min(1.0f, Math.Abs(value)); }}
        [NonSerialized]
        float m_tolerance;
        // This value is used to determine if we should chose an alternative option to the optimal option for the main objective.
        // e.g. If [Alternative]'s MainObj performance disadvantage is with Tolerance AND [Alternative]'s general performance advantage is higher than Tolerance.
        // Must be positive and between 0 & 1.
        public float Tolerance { get { return m_tolerance; } set { m_tolerance = Math.Min(1.0f, Math.Abs(value)); } }

        public ObjectiveBias(float weight, float tolerance) 
        {
            Weight = weight;
            m_tolerance = Math.Min(1.0f, Math.Abs(tolerance));
        }
    }

}


