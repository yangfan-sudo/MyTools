using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolygonAreaConfig))]
public class PolygonAreaConfigInspector : Editor
{
    private const string OriginEditorStateTitle = "Origin";
    private const string DirEditorStateTitle = "Direction";

    private Dictionary<string, bool> m_EditorState = new Dictionary<string, bool>();

    private const int TestPointCount = 1000;
    private List<Vector3> testPoints = new List<Vector3>(TestPointCount);
    private float testPointsSize = 0.1f;

    void OnEnable()
    {
        m_EditorState.Clear();
        m_EditorState.Add(OriginEditorStateTitle, false);
        m_EditorState.Add(DirEditorStateTitle, false);

        testPoints.Clear();
    }

    void OnDisable()
    {
        m_EditorState.Clear();

        testPoints.Clear();
    }

    #region Utils

    private void GetProperty(SerializedObject serializedObject,
        out SerializedProperty vertexesProp, out SerializedProperty trianglesProp,
        out SerializedProperty triangleAreasProp, out SerializedProperty originProp,
        out SerializedProperty dirProp, out SerializedProperty areaProp, out SerializedProperty offsetProp,
        out SerializedProperty areaNameProp, out SerializedProperty foldoutProp,
        out SerializedProperty pSizeProp, out SerializedProperty colorProp, out SerializedProperty vOffsetProp,
        out SerializedProperty dirLineLengthProp, out SerializedProperty dirLineWidthProp)
    {
        var polygonAreaProp = serializedObject.FindProperty("m_Area");

        vertexesProp = polygonAreaProp.FindPropertyRelative("Vertexes");
        trianglesProp = polygonAreaProp.FindPropertyRelative("Triangles");
        triangleAreasProp = polygonAreaProp.FindPropertyRelative("TriangleAreas");
        
        originProp = polygonAreaProp.FindPropertyRelative("m_Origin");
        dirProp = polygonAreaProp.FindPropertyRelative("m_Direction");
        areaProp = polygonAreaProp.FindPropertyRelative("m_Area");
        offsetProp = polygonAreaProp.FindPropertyRelative("m_Offset");

        areaNameProp = serializedObject.FindProperty("m_AreaName");
        foldoutProp = serializedObject.FindProperty("m_Foldout");
        pSizeProp = serializedObject.FindProperty("m_pSize");
        colorProp = serializedObject.FindProperty("m_AreaColor");
        vOffsetProp = serializedObject.FindProperty("m_AreaVertialOffsetBase");
        dirLineLengthProp = serializedObject.FindProperty("m_DirLineLength");
        dirLineWidthProp = serializedObject.FindProperty("m_DirLineWidth");
    }

    private static Plane GetPlane(Vector3 origin)
    {
        return new Plane(-PolygonArea.Normal, origin);
    }

    private void CheckEditorStates(string key_)
    {
        foreach (var key in m_EditorState.Keys.ToList())
        {
            if (key != key_)
                m_EditorState[key] = false;
        }
    }

    #endregion

    #region Inspector

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        this.ShowScriptInfo();

        GUILayout.Space(5);
        DrawArea();
        GUILayout.Space(5);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawArea()
    {
        GetProperty(serializedObject, out var vertexesProp, out var trianglesProp, out var triangleAreasProp,
            out var originProp, out var dirProp, out var areaProp, out var offsetProp, out var areaNameProp, out var foldoutProp,
            out var pSizeProp, out var colorProp, out var vOffsetProp, out var dirLineLengthProp, out var dirLineWidthProp);


        EditorGUILayout.PropertyField(areaNameProp, new GUIContent("区域名称"));
        EditorGUILayout.PropertyField(colorProp, new GUIContent("区域颜色"));
        EditorGUILayout.PropertyField(vOffsetProp, new GUIContent("海拔最低值"));
        EditorGUILayout.PropertyField(dirLineLengthProp, new GUIContent("方向指示线长度"));
        EditorGUILayout.PropertyField(dirLineWidthProp, new GUIContent("方向指示线粗度"));
        GUILayout.Space(5);

        var foldoutStyle = new GUIStyle(EditorStyles.foldout) { fontSize = 18, stretchHeight = true };

        foldoutProp.boolValue = EditorGUILayout.Foldout(foldoutProp.boolValue, areaNameProp.stringValue, foldoutStyle);
        if (!foldoutProp.boolValue)
        {
            testPoints.Clear();
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        DrawPointEditorProperty(originProp,"原点",30, OriginEditorStateTitle, () =>
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView)
                sceneView.LookAt(originProp.vector3Value, sceneView.rotation);
        });

        DrawPointEditorProperty(dirProp, "方向", 30, DirEditorStateTitle, () =>
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView)
                sceneView.LookAt(originProp.vector3Value, sceneView.rotation);
        });
        
        EditorGUILayout.BeginHorizontal(GUIStyleExtend.ColorHelpBox(new Color32(255, 128, 0, 70)));
        EditorGUILayout.LabelField("总面积", GUILayout.Width(50));
        GUI.enabled = false;
        EditorGUILayout.FloatField("", areaProp.floatValue);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        Plane p = GetPlane(originProp.vector3Value);

        int insertIndex = -1, delIndex = -1;
        for (int i = 0; i < vertexesProp.arraySize - 1; ++i)
        {
            var vProp = vertexesProp.GetArrayElementAtIndex(i);
            DrawVertex(vProp, offsetProp, p, i.ToString(), out var insert, out var del);
            insertIndex = insert ? i : insertIndex;
            delIndex = del ? i : delIndex;
        }

        offsetProp.vector3Value = Vector3.zero;

        if (insertIndex >= 0)
        {
            vertexesProp.InsertArrayElementAtIndex(insertIndex + 1);
            Vector3 v1 = vertexesProp.GetArrayElementAtIndex(insertIndex).vector3Value;
            Vector3 v2 = vertexesProp.GetArrayElementAtIndex(insertIndex + 2).vector3Value;
            vertexesProp.GetArrayElementAtIndex(insertIndex + 1).vector3Value = (v1 + v2) / 2.0f;
        }

        if (delIndex >= 0)
        {
            vertexesProp.DeleteArrayElementAtIndex(delIndex);
        }

        if (vertexesProp.arraySize == 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Add Points"), GUILayout.Height(20)))
            {
                if (vertexesProp.arraySize < 4)
                {
                    vertexesProp.ClearArray();
                    for (int i = 0; i < 4; ++i)
                    {
                        vertexesProp.InsertArrayElementAtIndex(vertexesProp.arraySize);
                        var prop = vertexesProp.GetArrayElementAtIndex(vertexesProp.arraySize - 1);
                        prop.vector3Value = SceneView.lastActiveSceneView?.pivot ?? Vector3.zero;
                    }
                        
                }
                else
                {
                    vertexesProp.InsertArrayElementAtIndex(vertexesProp.arraySize - 1);
                }
                    
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        areaProp.floatValue = CalculateTriangles(originProp, vertexesProp, trianglesProp, triangleAreasProp);

        CheckVertexesArray(vertexesProp);

        DrawRandomTest(vertexesProp, trianglesProp, triangleAreasProp, areaProp, pSizeProp);
    }

    private void DrawPointEditorProperty(SerializedProperty pointProp, string title, float titleWidth,
        string editorStateTitle, System.Action offToOnAction = null)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginHorizontal(GUIStyleExtend.ColorHelpBox(new Color32(255, 128, 0, 70)));
        EditorGUILayout.LabelField(title, GUILayout.Width(titleWidth));
        GUI.enabled = false;
        EditorGUILayout.Vector3Field("", pointProp.vector3Value);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        bool isEditorOn = GUILayout.Toggle(m_EditorState[editorStateTitle], "编辑", "Button", GUILayout.Width(40), GUILayout.Height(30));

        if (!m_EditorState[editorStateTitle] && isEditorOn) // 按钮状态由关到开
        {
            offToOnAction?.Invoke();
            SceneView.RepaintAll();
        }
        else if (m_EditorState[editorStateTitle] && !isEditorOn) // 按钮状态由开到关
        {
            SceneView.RepaintAll();
        }

        if (isEditorOn)
            CheckEditorStates(editorStateTitle);

        m_EditorState[editorStateTitle] = isEditorOn;

        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawVertex(SerializedProperty prop, SerializedProperty offsetProp, Plane plane, string title,
        out bool insert, out bool del)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField(title, GUILayout.Width(30));

        Vector3 value = prop.vector3Value;

        GUI.enabled = false;
        value = EditorGUILayout.Vector3Field("", value);
        GUI.enabled = true;

        var sceneView = SceneView.lastActiveSceneView;

        if (GUILayout.Button(EditorGUIUtility.IconContent("ClothInspector.ViewValue"), GUILayout.Width(30)))
        {
            sceneView?.LookAt(value, sceneView.rotation);
        }


        insert = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(30));

        del = GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(30));

        value = plane.GetPointOnPlane(value);
        
        prop.vector3Value = value + offsetProp.vector3Value;

        EditorGUILayout.EndHorizontal();
    }

    private float CalculateTriangles(SerializedProperty originProp, SerializedProperty vertexesProp,
        SerializedProperty trianglesProp, SerializedProperty triangleAreasProp)
    {
        
        Vector3[] vertexes = new Vector3[vertexesProp.arraySize];
        float area = 0;
        for (int i = 0; i < vertexesProp.arraySize; ++i)
        {
            vertexes[i] = vertexesProp.GetArrayElementAtIndex(i).vector3Value;
        }

        originProp.vector3Value = PolygonAreaExtend.GetCentroid(vertexes.ToList());

        Undo.RecordObject(((MonoBehaviour) target).gameObject, "Position");
        ((MonoBehaviour) target).transform.position = originProp.vector3Value;

        PolygonHelper.Triangulate(vertexes, -GetPlane(originProp.vector3Value).normal, out var triangles);

        if (triangles.Count != trianglesProp.arraySize)
        {
            trianglesProp.ClearArray();
            for (int i = 0; i < triangles.Count; ++i)
            {
                trianglesProp.InsertArrayElementAtIndex(i);
            }
        }

        if (triangles.Count != triangleAreasProp.arraySize)
        {
            triangleAreasProp.ClearArray();
            for (int i = 0; i < triangles.Count; ++i)
            {
                triangleAreasProp.InsertArrayElementAtIndex(i);
            }
        }
        
        for (int i = 0; i < triangles.Count; ++i)
        {
            var prop = trianglesProp.GetArrayElementAtIndex(i);
            var tri = triangles[i];
            prop.FindPropertyRelative("x").vector3Value = tri.x;
            prop.FindPropertyRelative("y").vector3Value = tri.y;
            prop.FindPropertyRelative("z").vector3Value = tri.z;

            var areaProp = triangleAreasProp.GetArrayElementAtIndex(i);
            areaProp.floatValue = tri.CalcTriangleArea();
            area += areaProp.floatValue;
        }
        
        return area;
    }

    private void CheckVertexesArray(SerializedProperty vertexesProp)
    {
        if (vertexesProp.arraySize < 4)
            vertexesProp.ClearArray();
        else
            vertexesProp.GetArrayElementAtIndex(vertexesProp.arraySize - 1).vector3Value =
                vertexesProp.GetArrayElementAtIndex(0).vector3Value;
    }

    private void DrawRandomTest(SerializedProperty vertexesProp, SerializedProperty trianglesProp,
        SerializedProperty triangleAreasProp, SerializedProperty areaProp, SerializedProperty pointSizeProp)
    {
        if (areaProp.floatValue <= 0 || Mathf.Approximately(areaProp.floatValue, 0))
            return;

        EditorGUILayout.BeginHorizontal();

        bool isTest = GUILayout.Button("测试");
        bool isClear = GUILayout.Button("清除");
        

        GUILayout.FlexibleSpace();
        GUILayout.Label("随机点大小");

        pointSizeProp.floatValue = EditorGUILayout.FloatField(pointSizeProp.floatValue, GUILayout.Width(100));
        pointSizeProp.floatValue = Mathf.Clamp(pointSizeProp.floatValue, 0.01f, 1.0f);

        bool isRefresh = GUILayout.Button("刷新");

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


        if (isTest)
        {
            if (vertexesProp.arraySize > 3)
            {
                PolygonArea area = new PolygonArea()
                {
                    Vertexes = new List<Vector3>(),
                    Triangles = new List<ThreeVector3>(),
                    TriangleAreas = new List<float>(),
                    Area = areaProp.floatValue,
                };
                for (int i = 0; i < vertexesProp.arraySize; ++i)
                {
                    area.Vertexes.Add(vertexesProp.GetArrayElementAtIndex(i).vector3Value);
                }

                for (int i = 0; i < trianglesProp.arraySize; ++i)
                {
                    var triProp = trianglesProp.GetArrayElementAtIndex(i);
                    var x = triProp.FindPropertyRelative("x").vector3Value;
                    var y = triProp.FindPropertyRelative("y").vector3Value;
                    var z = triProp.FindPropertyRelative("z").vector3Value;
                    area.Triangles.Add(new ThreeVector3(x, y, z));

                    area.TriangleAreas.Add(triangleAreasProp.GetArrayElementAtIndex(i).floatValue);
                }

                testPoints.Clear();
                for (int i = 0; i < TestPointCount; ++i)
                {
                    Vector3 p = area.RandomPoint();
                    testPoints.Add(p);
                }
                testPointsSize = pointSizeProp.floatValue;
                SceneView.RepaintAll();
            }
        }

        if (isClear)
        {
            testPoints.Clear();
            SceneView.RepaintAll();
        }
        

        if (isRefresh)
        {
            testPointsSize = pointSizeProp.floatValue;
            SceneView.RepaintAll();
        }

        
    }

    #endregion

    #region SceneView

    void OnSceneGUI()
    {
        Tools.current = Tool.None;
        serializedObject.Update();
        HandlesDrawArea();
        serializedObject.ApplyModifiedProperties();
        DrawRandomTest();
        Repaint();
    }

    private void HandlesDrawArea()
    {
        GetProperty(serializedObject, out var vertexesProp, out var trianglesProp, out _,
            out var originProp, out var dirProp, out _, out var offsetProp, out _, 
            out var foldoutProp, out _, out var colorProp, out var vOffsetProp, 
            out var dirLineLengthProp, out var dirLineWidthProp);

        if (!foldoutProp.boolValue)
            return;

        
        Color color = colorProp.colorValue;
        color.a = 1.0f;
        
        var plane = GetPlane(originProp.vector3Value);

        if (m_EditorState[OriginEditorStateTitle])
        {
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, dirProp.vector3Value);
            Vector3 value = originProp.vector3Value;
            EditorGUI.BeginChangeCheck();
            value = Handles.PositionHandle(value, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                offsetProp.vector3Value = value - originProp.vector3Value;
            }
        }

        if (m_EditorState[DirEditorStateTitle])
        {
            Vector3 origin = originProp.vector3Value;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, dirProp.vector3Value);
            rotation = Handles.RotationHandle(rotation, origin);
            Vector3 dir = rotation * Vector3.forward;

            Vector3 zeroAngleDir = Quaternion.AngleAxis(0, -plane.normal) * Vector3.forward;
            Vector3 dirOnPlane = (plane.GetPointOnPlane(origin + dir) - origin)
                .normalized;
            float angle = Vector3.SignedAngle(zeroAngleDir, dirOnPlane, -plane.normal);
            dir = Quaternion.AngleAxis(angle, -plane.normal) * Vector3.forward;
            dirProp.vector3Value = dir;
        }

        DrawArrow(originProp.vector3Value - plane.normal * 0.2f, 
            originProp.vector3Value + dirProp.vector3Value * dirLineLengthProp.floatValue - plane.normal * 0.2f, 
            plane.normal, dirLineWidthProp.floatValue, color);

        Handles.color = color;

        Vector3[] vertexes = new Vector3[vertexesProp.arraySize];
        for (int i = 0; i < vertexesProp.arraySize; ++i)
        {
            if (i == vertexesProp.arraySize - 1)
                vertexes[i] = vertexes[0];
            else
                vertexes[i] = HandlesDrawVector3(vertexesProp.GetArrayElementAtIndex(i), i, plane);
            if (i > 0)
                Handles.DrawAAPolyLine(5.0f, vertexes[i], vertexes[i - 1]);
        }

        for (int i = 1; i < vertexesProp.arraySize; ++i)
        {
            Vector3 v1 = vertexesProp.GetArrayElementAtIndex(i).vector3Value;
            Vector3 v2 = vertexesProp.GetArrayElementAtIndex(i - 1).vector3Value;
            Vector3 v3 = v1 - plane.normal * (Vector3.Dot(v1, plane.normal) + vOffsetProp.floatValue);
            Vector3 v4 = v2 - plane.normal * (Vector3.Dot(v2, plane.normal) + vOffsetProp.floatValue);
            Handles.color = new Color(color.r, color.g, color.b, 0.08f);
            Handles.DrawAAConvexPolygon(v1, v2, v4, v3, v1);
        }

        const float pixel = 3.0f;

        for (int i = 0; i < trianglesProp.arraySize; ++i)
        {
            var triProp = trianglesProp.GetArrayElementAtIndex(i);
            var x = triProp.FindPropertyRelative("x").vector3Value;
            var y = triProp.FindPropertyRelative("y").vector3Value;
            var z = triProp.FindPropertyRelative("z").vector3Value;

            Handles.color = Color.grey;
            Handles.DrawDottedLine(x, y, pixel);
            Handles.DrawDottedLine(y, z, pixel);
            Handles.DrawDottedLine(z, x, pixel);
            Handles.color = new Color(color.r, color.g, color.b, 0.08f);
            Handles.DrawAAConvexPolygon(x, y, z, x);
        }
    }

    private Vector3 HandlesDrawVector3(SerializedProperty prop, int index, Plane plane)
    {
        Vector3 value = prop.vector3Value;
        float size = HandleUtility.GetHandleSize(value) * 0.2f;
        Vector3 snap = Vector3.one * 0.5f;
        EditorGUI.BeginChangeCheck();
        value = plane.GetPointOnPlane(Handles.FreeMoveHandle(value, Quaternion.identity, size, snap, Handles.SphereHandleCap));
        if (EditorGUI.EndChangeCheck())
        {
            prop.vector3Value = value;
        }

        Handles.Label(value, index.ToString(), new GUIStyle(EditorStyles.whiteLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter });
        
        return value;
    }

    private void DrawRandomTest()
    {
        Handles.color = new Color32(128, 0, 255, 128);
        foreach (var point in testPoints)
        {
            Handles.SphereHandleCap(0, point, Quaternion.FromToRotation(Vector3.forward, Vector3.up), testPointsSize,
                EventType.Repaint);
        }
    }

    private static void DrawArrow(Vector3 startPos, Vector3 endPos, Vector3 inNormal, float width, Color color)
    {
        float mag = Vector3.Distance(startPos, endPos);
        width = Mathf.Clamp(width, 0.001f, mag / 2.0f);
        Vector3 dir = (endPos - startPos).normalized;
        startPos = startPos + dir * width;
        endPos = endPos - dir * width;

        var r1 = Quaternion.AngleAxis(-45, inNormal);
        var r2 = Quaternion.AngleAxis(45, inNormal);

        DrawLine(startPos, endPos, inNormal, width, color);
        var arrow1 = endPos + r1 * (startPos - endPos).normalized * mag / 15.0f;
        var arrow2 = endPos + r2 * (startPos - endPos).normalized * mag / 15.0f;
        DrawLine(endPos, arrow1, inNormal, width, color);
        DrawLine(endPos, arrow2, inNormal, width, color);

    }

    private static void DrawLine(Vector3 startPos, Vector3 endPos, Vector3 inNormal, float width, Color color)
    {
        float mag = Vector3.Distance(startPos, endPos);
        width = Mathf.Clamp(width, 0.001f, mag / 2.0f);
        int slice = Mathf.CeilToInt(mag / width) * 3;
        Vector3 dir = (endPos - startPos).normalized;
        Handles.color = color;
        for (int i = 0; i <= slice; ++i)
        {
            Vector3 pos = startPos + dir * mag / slice * i;
            Handles.DrawSolidDisc(pos, inNormal, width);
        }
    }

    #endregion


    #region Gizmo

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected)]
    private static void DrawGizmo(PolygonAreaConfig scr, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) == GizmoType.Selected)
            return;

        if ((gizmoType & GizmoType.InSelectionHierarchy) != GizmoType.InSelectionHierarchy)
        {
            var count = Selection.activeGameObject?.transform.parent?.GetComponentsInChildren<PolygonAreaConfig>().Length ?? 0;
            var selfCount = scr.GetComponents<PolygonAreaConfig>().Length;
            if (count <= selfCount)
                return;
        }

        GetValue(scr, out var area, out var origin, out var dir, out var color, 
            out var vOffset,out var dirLineLength, out var dirLineWidth);

        var plane = GetPlane(origin);

        Handles.color = color;
        color.a = 1.0f;

        if (area.Vertexes == null)
            return;
        Handles.DrawAAPolyLine(5.0f, area.Vertexes.ToArray());

        DrawArrow(origin - plane.normal * 0.2f, origin + dir * dirLineLength - plane.normal * 0.2f,
            plane.normal, dirLineWidth, color);

        color.a = 0.04f;
        Handles.color = color;

        Handles.DrawAAConvexPolygon(area.Vertexes.ToArray());


        for (int i = 1; i < area.Vertexes.Count; ++i)
        {
            Vector3 v1 = area.Vertexes[i];
            Vector3 v2 = area.Vertexes[i - 1];
            Vector3 v3 = v1 - plane.normal * (Vector3.Dot(v1, plane.normal) + vOffset);
            Vector3 v4 = v2 - plane.normal * (Vector3.Dot(v2, plane.normal) + vOffset);
            color.a = 0.04f;
            Handles.color = color;
            Handles.DrawAAConvexPolygon(v1, v2, v4, v3, v1);
        }
    }

    private static void GetValue(PolygonAreaConfig scr, out PolygonArea area, out Vector3 origin, out Vector3 dir, 
        out Color color, out float vOffset, out float dirLineLength,out float dirLineWidth)
    {
        var type = scr.GetType();
        area = (PolygonArea)type.GetFieldAll("m_Area").GetValue(scr);
        color = (Color)type.GetFieldAll("m_AreaColor").GetValue(scr);
        vOffset = (float)type.GetFieldAll("m_AreaVertialOffsetBase").GetValue(scr);
        dirLineLength = (float)type.GetFieldAll("m_DirLineLength").GetValue(scr);
        dirLineWidth = (float)type.GetFieldAll("m_DirLineWidth").GetValue(scr);

        var polygonType = area.GetType();
        origin = (Vector3)polygonType.GetFieldAll("m_Origin").GetValue(area);
        dir = (Vector3)polygonType.GetFieldAll("m_Direction").GetValue(area);
        

    }

    #endregion

}
