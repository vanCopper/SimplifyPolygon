using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplifyMesh
{
    public Mesh originMesh;
    public List<SimplifyTriangle> triangles = new List<SimplifyTriangle>();
    public List<SimplifyVertex> vertices = new List<SimplifyVertex>();
    public SimplifyMesh(Mesh mesh)
    {
        originMesh = mesh;
        InitMesh();
    }
    
    public Mesh GetSimplifyMesh()
    {
        return null;
    }

    private void InitMesh()
    {
        Vector3[] originVertices = originMesh.vertices;
        int[] originTriangles = originMesh.triangles;

        for(int i = 0; i < originVertices.Length; i++)
        {
            Vector3 vexter = originVertices[i];
            vertices.Add(new SimplifyVertex(vexter, i));
        }

        int v0;
        int v1;
        int v2;
        for(int j = 0; j < originTriangles.Length; j+=3)
        {
            v0 = originTriangles[j];
            v1 = originTriangles[j + 1];
            v2 = originTriangles[j + 2];
            triangles.Add(new SimplifyTriangle(vertices[v0], vertices[v1], vertices[v2]));

            vertices[v0].AppendNeighbor(vertices[v1]);
            vertices[v0].AppendNeighbor(vertices[v2]);
            vertices[v1].AppendNeighbor(vertices[v2]);
        }

        Debug.Log("InitMesh");
    }

    private float ComputeEdgeCollapseCost(SimplifyVertex u, SimplifyVertex v)
    {
        int i;
        float edgelength = (u.position - v.position).sqrMagnitude;
        float curvature = 0; // 曲率

        List<SimplifyTriangle> sides;

        foreach(SimplifyTriangle simplifyTriangle in u.triangles)
        {

        }

        return 1.0f;
    }

}
