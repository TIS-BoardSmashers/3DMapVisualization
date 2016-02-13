using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FileBrowser))]
public class FileBrowserInspector : Editor
{
    public bool customRects = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (Application.isPlaying)
        {
            EditorGUILayout.Separator();
            GUI.backgroundColor = new Color(0.8f, 1, 0.8f);
            if (GUILayout.Button("Show/Hide"))
            {
                FileBrowser b = (FileBrowser)serializedObject.targetObject;
                if (!b.isShowing)
                    b.Show("", null);
                else b.Hide();
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.Separator();
        DrawProperty("skin", "Skin:");
        EditorGUILayout.Separator();
        DrawProperty("SelectEventName", "Select Event Name:");
        DrawProperty("CancelEventName", "Cancel Event Name:");
        DrawProperty("FileChangedEventName", "File Change Event Name:");
        EditorGUILayout.Separator();
        DrawProperty("DefaultDirectory", "Default Directory:");
        DrawProperty("windowName", "Window Name:");
        DrawProperty("redLabel", "Tip Label:");
        DrawProperty("drawIcons", "Draw File Icons");
        if (serializedObject.FindProperty("drawIcons").boolValue)
        {
            DrawProperty("scaleMode", "Scale Mode:");
            DrawProperty("folderIcon", "Folder Icon:");
            DrawProperty("defaultIcon", "Default Icon:");
            DrawProperty("fileIcons", "File Icons");
            DrawProperty("iconsPos", "Icon Offset");
        }
        DrawProperty("draggable", "Draggable");
        if (serializedObject.FindProperty("draggable").boolValue)
        DrawProperty("dragRect", "Draggable Rect");
        DrawProperty("customRects", "Custom Positions");
        if (serializedObject.FindProperty("customRects").boolValue)
        {
            DrawProperty("windowPos", "Window");
            DrawProperty("currentDirTextPos", "Current Directory Text");
            DrawProperty("fileBoxPos", "Files Box");
            DrawProperty("scrollViewPos", "Scroll View");
            DrawProperty("filePos", "Files List");
            DrawProperty("upButtonPos", "Up Button");
            DrawProperty("homeButtonPos", "Home Button");
            DrawProperty("cancelButtonPos", "Cancel Button");
            DrawProperty("okButtonPos", "OK Button");
            DrawProperty("redLabelPos", "Tip Label");
        }
        DrawProperty("upButton", "Up Button Texture");
        DrawProperty("homeButton", "Home Button Texture");
    }

    void DrawProperty(string name, string label)
    {
        SerializedProperty prop = serializedObject.FindProperty(name);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(prop, new GUIContent(label), true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
        EditorGUIUtility.LookLikeControls();
    }
}