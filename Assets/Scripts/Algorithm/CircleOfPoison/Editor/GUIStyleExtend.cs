using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class
    GUIStyleExtend
{
    public static Dictionary<string, Texture2D> m_Caches = new Dictionary<string, Texture2D>();

    public static Texture2D MakeTex(int width, int height, Color color)
    {
        var key = GenerateTextureKey(width, height, color);
        if (m_Caches.ContainsKey(key))
            return m_Caches[key];
        var tex = new Texture2D(width, height);
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                tex.SetPixel(i, j, color);
            }
        }

        tex.Apply(true, true);
        m_Caches.Add(key, tex);
        return tex;
    }

    private static string GenerateTextureKey(int width, int height, Color color)
    {
        return $"{width}{height}{Mathf.FloorToInt(color.r * 255)}{Mathf.FloorToInt(color.g * 255)}{Mathf.FloorToInt(color.b * 255)}{Mathf.FloorToInt(color.a * 255)}";
    }

    public static GUIStyle ColorHelpBox(Color bgColor)
    {
        return new GUIStyle(EditorStyles.helpBox)
            { normal = new GUIStyleState() { background = GUIStyleExtend.MakeTex(600, 1, bgColor) } };
    }
}
