using UnityEngine;
using UnityEditor;
using BlindChase.Utility;
using UnityEngine.UIElements;

namespace BlindChase.Utility 
{
    // Inspector setup for RangeDisplayMasks to auto-generate some range tiles
    [CustomEditor(typeof(RangeDisplayMasks))]
    public class RangeDisplayMasksEditor : Editor
    {
        int maxRange = 0;

        SerializedObject m_editorObject;

        void OnEnable()
        {
            m_editorObject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RangeDisplayMasks script = target as RangeDisplayMasks;
            maxRange = EditorGUILayout.IntField("Max Square Range to Generate:", maxRange);

            if (GUILayout.Button("Generate Square Radius Masks", GUILayout.Height(20)))
            {
                script.GenerateSquareRadiusMaps(maxRange);
                m_editorObject.ApplyModifiedProperties();
            }
        }
    }
}

