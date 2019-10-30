using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleRoyaleGameArea))]
public class BattleRoyaleGameAreaEditor : Editor
{
    private void GetProperties(SerializedObject serializedObject, out SerializedProperty prepareareas,
        out SerializedProperty bornareas, out SerializedProperty gamearea)
    {
        prepareareas = serializedObject.FindProperty("m_PrepareArea");
        bornareas = serializedObject.FindProperty("m_BornArea");
        gamearea = serializedObject.FindProperty("m_GameArea");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        BattleRoyaleGameArea mscript = (target as BattleRoyaleGameArea);
        GetProperties(serializedObject, out var prepareareas, out var bornareas, out var gamearea);

        this.ShowScriptInfo();

        var titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 20;
        var titleHeight = GUILayout.Height(25);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("❉ 战斗区域", titleStyle, titleHeight);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        string title = "EMPTY";
        if (gamearea.objectReferenceValue is PolygonAreaConfig areaConfig)
        {
            title = areaConfig.AreaName;
        }
        int delIndex = -1;
        DrawArea(gamearea, title, out var del);

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("❉ 准备区域", titleStyle, titleHeight);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        for (int i = 0; i < prepareareas.arraySize ; i++)
        {
            var element = prepareareas.GetArrayElementAtIndex(i);
            string areaName = "EMPTY";
            if (element.objectReferenceValue is PolygonAreaConfig tmpareaConfig)
            {
                areaName = tmpareaConfig.AreaName == string.Empty ? areaName: tmpareaConfig.AreaName;
            }

            string tmptitle = "prepare:"+ i;
            DrawArea(element, tmptitle,  out var deltmp);
            delIndex = deltmp ? i : delIndex;

        }
       
        if (delIndex >= 0)
        {
            GameObject obj = mscript.PrepareArea[delIndex].gameObject;
            mscript.PrepareArea.RemoveAt(delIndex);
            GameObject.DestroyImmediate(obj);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Add Area"), GUILayout.Height(20)))
        {
            prepareareas.InsertArrayElementAtIndex(prepareareas.arraySize);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("❉ 出生区", titleStyle, titleHeight);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        delIndex = -1;
        for (int i = 0; i < bornareas.arraySize; i++)
        {
            var element = bornareas.GetArrayElementAtIndex(i);
            string areaName = "EMPTY";
            if (element.objectReferenceValue is PolygonAreaConfig tmpareaConfig)
            {
                areaName = tmpareaConfig.AreaName == string.Empty ? areaName : tmpareaConfig.AreaName;
            }

            string tmptitle = "born:"+ i;
            DrawArea(element, tmptitle, out var deltmp);
            delIndex = deltmp ? i : delIndex;

        }

        if (delIndex >= 0)
        {
            GameObject obj = mscript.BornArea[delIndex].gameObject;
            mscript.BornArea.RemoveAt(delIndex);
            GameObject.DestroyImmediate(obj);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Add Area"), GUILayout.Height(20)))
        {
            bornareas.InsertArrayElementAtIndex(bornareas.arraySize);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawArea(SerializedProperty element, string title,out bool del)
    {
        EditorGUILayout.BeginHorizontal(GUIStyleExtend.ColorHelpBox(new Color32(255, 128, 0, 70)));
        EditorGUILayout.PropertyField(element, new GUIContent(title));
        del = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();
    }   
}
