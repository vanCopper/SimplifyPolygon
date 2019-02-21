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

    public static float Xor(Vector3 v0, Vector3 v1)
    {
        return v0.x * v1.x + v0.y * v0.y + v0.z * v1.z;
    }
   
}
