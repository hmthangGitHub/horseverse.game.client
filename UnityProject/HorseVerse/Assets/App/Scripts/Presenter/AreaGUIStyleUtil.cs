using UnityEngine;

public static class AreaGUIStyleUtil
{
    public static GUIStyle CreateGs(Color color)
    {
        return new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = MakeTex(1, 1, color)
            }
        };
    }
    
    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    public static void TextFieldWithLabel(string label, ref string  text)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        text = GUILayout.TextField(text);
        GUILayout.EndHorizontal();
    }
}