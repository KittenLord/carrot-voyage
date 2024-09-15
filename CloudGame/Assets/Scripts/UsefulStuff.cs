using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static class UsefulStuff
{
    public static Vector2 v2(this Vector3 v) => new Vector2(v.x, v.y);
    public static List<T> Reverted<T>(this List<T> list)
    {
        List<T> nl = new();
        for(int i = list.Count - 1; i >= 0; i--)
        {
            nl.Add(list[i]);
        }
        return nl;
    }
}

public static class HexColor
{
    public static Color Get(uint hex)
    {
        float r = ((hex >> (8 * 3)) & 0xff) / 255.0f;
        float g = ((hex >> (8 * 2)) & 0xff) / 255.0f;
        float b = ((hex >> (8 * 1)) & 0xff) / 255.0f;
        float a = ((hex >> (8 * 0)) & 0xff) / 255.0f;
        return new Color(r, g, b, a);
    }
}
