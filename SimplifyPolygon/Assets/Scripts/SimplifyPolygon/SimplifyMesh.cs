﻿using UnityEngine;
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
        ComputeAllEdgeCollapseCosts();

        while(true)
        {
            int vertexCount = vertices.Count;
            if (vertexCount == 0) break;

            SimplifyVertex mn = MiniCostEdge();

            Collapse(mn, mn.collapse);
            vertices.Remove(mn);
        }

        triangles.RemoveAll((triangle) => {
            return triangle.isRemoved;
        });



        return null;
    }

    private void InitMesh()
    {
        Vector3[] originVertices = originMesh.vertices;
        int[] originTriangles = originMesh.triangles;

        for (int i = 0; i < originVertices.Length; i++)
        {
            Vector3 vexter = originVertices[i];
            vertices.Add(new SimplifyVertex(vexter, i));
        }

        int v0;
        int v1;
        int v2;
        for (int j = 0; j < originTriangles.Length; j += 3)
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

    private SimplifyVertex MiniCostEdge()
    {
        SimplifyVertex mn = vertices[0];
        foreach(SimplifyVertex v in vertices)
        {
            if(v.cost < mn.cost)
            {
                mn = v;
            }
        }
        return mn;
    }

    private float ComputeEdgeCollapseCost(SimplifyVertex u, SimplifyVertex v)
    {
        float edgelength = (u.position - v.position).sqrMagnitude;
        float curvature = 0; // 曲率

        List<SimplifyTriangle> sides = new List<SimplifyTriangle>();

        // 查找uv为边的三角形
        foreach (SimplifyTriangle simplifyTriangle in u.triangles)
        {
            if (simplifyTriangle.Contains(v))
            {
                sides.Add(simplifyTriangle);
            }
        }

        foreach (SimplifyTriangle triangle in u.triangles)
        {
            float mincurv = 1;
            foreach (SimplifyTriangle sTriangle in sides)
            {
                float dotprod = VertexUtils.Xor(triangle.normal, sTriangle.normal);
                mincurv = Mathf.Min(mincurv, (1 - dotprod) / 2.0f);
            }

            curvature = Mathf.Max(curvature, mincurv);
        }

        return edgelength * curvature;
    }

    private void ComputeEdgeCostAtVertex(SimplifyVertex v)
    {
        if (v.neighbors.Count == 0)
        {
            v.collapse = null;
            v.cost = -0.01f;
            return;
        }

        v.cost = 1000000.0f;
        v.collapse = null;

        foreach (SimplifyVertex nVertex in v.neighbors)
        {
            float cost = ComputeEdgeCollapseCost(v, nVertex);
            if (cost < v.cost)
            {
                v.collapse = nVertex;
                v.cost = cost;
            }
        }
    }

    private void ComputeAllEdgeCollapseCosts()
    {
        foreach(SimplifyVertex v in vertices)
        {
            ComputeEdgeCostAtVertex(v);
        }
    }

    // u->v 顶点v替换u 移除u
    private void Collapse(SimplifyVertex u, SimplifyVertex v)
    {
        if(v == null)
        {
            u.Remove();
            return;
        }

        List<SimplifyVertex> neighbors = new List<SimplifyVertex>(u.neighbors);
        List<SimplifyTriangle> triangles = new List<SimplifyTriangle>(u.triangles);

        foreach(SimplifyTriangle triangle in triangles)
        {
            if(triangle.Contains(v))// 移除包含uv边的三角形
            {
                triangle.Remove();
            }else
            {
                // 用v替换u 更新包含u但不包含v的三角形
                triangle.ReplaceSimplifyVertex(u, v);
            }
        }
        // 移除顶点u
        u.Remove();
        //重新计算u.neighbors顶点代价
        foreach(SimplifyVertex vertex in neighbors)
        {
            ComputeEdgeCostAtVertex(vertex);
        }
    }

}
