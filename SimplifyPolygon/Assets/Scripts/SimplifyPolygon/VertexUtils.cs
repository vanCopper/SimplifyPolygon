using UnityEngine;
using System.Collections;

public class VertexUtils
{
    public static float magnitude(Vector3 v)
    {
        return (float)Mathf.Sqrt(sqr(v.x) + sqr(v.y) + sqr(v.z));
    }

    public static float sqr(float a)
    {
        return a * a;
    }
   
}
