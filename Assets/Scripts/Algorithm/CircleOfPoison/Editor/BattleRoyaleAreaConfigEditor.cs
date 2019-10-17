using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleRoyaleAreaConfig))]
public class BattleRoyaleAreaConfigEditor : Editor
{
    private void GetProperties(SerializedObject serializedObject, out SerializedProperty centerPostion,
        out SerializedProperty poisonConfig)
    {
        centerPostion = serializedObject.FindProperty("m_Mapcenter");
        poisonConfig = serializedObject.FindProperty("m_ListPoisonData");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        this.ShowScriptInfo();
        GUILayout.Space(5);
        DrawBattleRoyaleAreaGUI();
        GUILayout.Space(5);

        serializedObject.ApplyModifiedProperties();
        //TO DO 清除这个Texture缓存应该由更好的地方，或者Texture不缓存用某个固定的Texture
        if(EditorApplication.isPlaying)
        {
            if(GUIStyleExtend.m_Caches.Count>0)
            {
                GUIStyleExtend.m_Caches.Clear();
            }
        }
        
    }
    BattleRoyaleAreaConfig m_config;
    private void DrawBattleRoyaleAreaGUI()
    {
        m_config = target as BattleRoyaleAreaConfig;
        GetProperties(serializedObject, out var centerPostion, out var poisonConfig);
        EditorGUILayout.PropertyField(centerPostion, new GUIContent("地图中心点"));
        GUILayout.Space(5);
        var titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 20;
        var titleHeight = GUILayout.Height(25);
        EditorGUILayout.LabelField("❉ 各阶段毒圈范围", titleStyle, titleHeight);
        GUILayout.Space(5);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        int insertIndex = -1, delIndex = -1;
        for (int i = 0; i < poisonConfig.arraySize; i++)
        {
            var element = poisonConfig.GetArrayElementAtIndex(i);
            Color color = Color.gray;

            if(i== lookIndex)
            {
                color = new Color(0, 201, 255, 0.6f);
            }
            DrawOnePoisonData(element, color, out var insert, out var del,out var look);
            insertIndex = insert ? i : insertIndex;
            delIndex = del ? i : delIndex;
            lookIndex = look ? i : lookIndex;
        }

        if (insertIndex >= 0)
        {
            poisonConfig.InsertArrayElementAtIndex(insertIndex + 1);
        }
        if (delIndex >= 0)
        {
            Undo.RecordObject(m_config, "取消删除");
            poisonConfig.DeleteArrayElementAtIndex(delIndex);
        }
        if (poisonConfig.arraySize == 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Add Points"), GUILayout.Height(20)))
            {
                poisonConfig.InsertArrayElementAtIndex(poisonConfig.arraySize);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        


    }
    void OnSceneGUI()
    {
        Tools.current = Tool.None;
        serializedObject.Update();
        HandleDrawBattleRoyaleAreaScene();
        serializedObject.ApplyModifiedProperties();
        Repaint();
    }
    private void DrawOnePoisonData(SerializedProperty element,Color color, out bool insert, out bool del,out bool look)
    {
        SerializedProperty radius = element.FindPropertyRelative("Radius");
        SerializedProperty durationtime = element.FindPropertyRelative("Durationtime");
        SerializedProperty reduceRadiusOneSecond = element.FindPropertyRelative("ReduceRadiusOneSecond");
        EditorGUILayout.BeginHorizontal(GUIStyleExtend.ColorHelpBox(color));
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(radius, new GUIContent("半径"));
        EditorGUILayout.PropertyField(durationtime, new GUIContent("等待缩圈时间"));
        EditorGUILayout.PropertyField(reduceRadiusOneSecond, new GUIContent("缩圈速度"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        insert = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(30));
        del = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(30));
        EditorGUILayout.EndVertical();
        look = GUILayout.Button(EditorGUIUtility.IconContent("ClothInspector.ViewValue"), GUILayout.Width(60), GUILayout.Height(40));
        EditorGUILayout.EndHorizontal();
    }
    private Color tmpcolor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.4f);
    private float maxrange = 600;
    private int lookIndex = 0;
    private void HandleDrawBattleRoyaleAreaScene()
    {
        GetProperties(serializedObject, out var centerPostion, out var poisonConfig);
        Handles.color = Color.white;
        HandlesDrawVector3(centerPostion);
        Handles.color = tmpcolor;
        if (poisonConfig.arraySize>0 )
        {
            SerializedProperty element;
            if (lookIndex>= poisonConfig.arraySize)
            {
                element = poisonConfig.GetArrayElementAtIndex(0);
            }else
            {
                element = poisonConfig.GetArrayElementAtIndex(lookIndex);
            }
            SerializedProperty radius = element.FindPropertyRelative("Radius");
            maxrange = (float)radius.intValue;
        }
        Handles.DrawSolidDisc(centerPostion.vector3Value, Vector3.up, maxrange);
    }

    private void HandlesDrawVector3(SerializedProperty prop)
    {
        Vector3 value = prop.vector3Value;
        float size = HandleUtility.GetHandleSize(value) * 0.2f;
        Vector3 snap = Vector3.one * 0.5f;
        EditorGUI.BeginChangeCheck();
        value = Handles.FreeMoveHandle(value, Quaternion.identity, size, snap, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            prop.vector3Value = value;
        }
        Handles.Label(value, "center", new GUIStyle(EditorStyles.whiteLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter });
        
    }
}
