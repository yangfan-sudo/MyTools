using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public static class EditorHelper
{
    public static void ShowScriptInfo(this Editor editor)
    {
        if (editor.target == null || editor.serializedObject == null)
            return;
        if (editor.serializedObject.isEditingMultipleObjects)
        {
            Type type = null;
            foreach (var obj in editor.serializedObject.targetObjects)
            {
                if (type == null)
                    type = obj.GetType();
                else if (type != obj.GetType())
                    return;
            }

            if (type == null)
                return;
            ShowScriptInfo(editor.target, editor.serializedObject.targetObjects[0]);
        }
        else
        {
            ShowScriptInfo(editor.target, editor.serializedObject.targetObject);
        }
    }

    private static void ShowScriptInfo(Object target, Object targetObject)
    {
        if (target == null || targetObject == null)
            return;

        MonoScript monoScript = null;
        if (target is MonoBehaviour monoTarget)
        {
            monoScript = MonoScript.FromMonoBehaviour(monoTarget);
        }
        else if (target is ScriptableObject scriptObjTarget)
        {
            monoScript = MonoScript.FromScriptableObject(scriptObjTarget);
        }

        if (monoScript == null)
            return;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", monoScript,targetObject.GetType(), false);
        GUI.enabled = true;

    }
}
