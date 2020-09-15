using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using BlindChase.Events;

[CustomEditor(typeof(BCGameEventListeners))]
public class BCGameEventListenersEditor : Editor
{
    SerializedProperty m_eventHandler;

    ReorderableList m_listenerList;
    private void OnEnable()
    {
        //Gets the wave property in WaveManager so we can access it. 
        m_eventHandler = serializedObject.FindProperty("m_eventListeners");

        //Initialises the ReorderableList. We are creating a Reorderable List from the "wave" property. 
        //In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
        m_listenerList = new ReorderableList(serializedObject, m_eventHandler, true, true, true, true);
        m_listenerList.drawHeaderCallback = DrawHeader;
        m_listenerList.drawElementCallback = DrawListenerItems;
    }

    void DrawListenerItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = m_listenerList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty gameEventRef = element.FindPropertyRelative("m_eventToListen");
        string prefabRefName = gameEventRef.displayName;
        EditorGUI.ObjectField(
            rect,
            gameEventRef,
            new GUIContent(prefabRefName));

        string name = $"Responses to event {gameEventRef?.objectReferenceValue?.name} :";
        // Display event responses as reorderable lists
        EditorGUILayout.PropertyField(
            element.FindPropertyRelative("m_responses"),
            new GUIContent(name));
    }

    void DrawHeader(Rect rect)
    {
        string name = "BCEventHandler";
        EditorGUI.LabelField(rect, name);
    }


    //This is the function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_listenerList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}