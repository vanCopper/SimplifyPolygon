using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplifyMesh
{
    /// <summary>
    ///  1. 搜集顶点、三角面和三角边的关系。 
    ///  2. 计算坍缩代价和坍缩目标，并排序。 
    ///  3. 替换坍缩代价最小的点，并重新计算相邻点的坍缩代价和坍缩目标，更新有序列表。 
    ///  4. 判断当前顶点数量是否大于目标数量，是则重复第3步。
    /// </summary>
    public Mesh originMesh;
    public List<SimplifyTriangle> triangles = new List<SimplifyTriangle>();
    public List<SimplifyVertex> vertices = new List<SimplifyVertex>();

    public int renderVerticesNum = 0; // 当前渲染的顶点数
    public int renderTrianglesNum = 0; // 当前渲染的面数

    public int originVerticesNum = 0; // 原网格顶点数
    public int originTrianglesNum = 0; // 原网格三角形数

    private int m_targetVerNum = 0;

    public SimplifyMesh(Mesh mesh)
    {
        originMesh = mesh;
        InitMesh();
    }

    public Mesh GetSimplifyMesh(int targetVerNum)
    {
        if (m_targetVerNum == targetVerNum) return null;

        m_targetVerNum = targetVerNum;
        ComputeAllEdgeCollapseCosts();

        while(true)
        {
            int vertexCount = vertices.Count;
            if (vertexCount == 0 || vertexCount <= m_targetVerNum) break;

            SimplifyVertex mn = MiniCostEdge();

            //if (mn.isRemoved || mn.cost > 1000000.0f) break;

            Collapse(mn, mn.collapse);
            vertices.Remove(mn);
        }

        triangles.RemoveAll((triangle) => {
            return triangle.isRemoved;
        });

        List<int> indices = new List<int>();
        List<Vector3> tVertices = new List<Vector3>();
        List<Vector2> tUVS = new List<Vector2>();
        Dictionary<int, int> idindex = new Dictionary<int, int>();

        int indexV = 0;
        foreach (SimplifyTriangle triangle in triangles)
        {
            if (!idindex.TryGetValue(triangle.v0.id, out indexV))
            {
                indexV = tVertices.Count;
                idindex.Add(triangle.v0.id, indexV);
                tVertices.Add(triangle.v0.position);
                tUVS.Add(triangle.v0.uv);
            }
            indices.Add(indexV);
            if (!idindex.TryGetValue(triangle.v1.id, out indexV))
            {
                indexV = tVertices.Count;
                idindex.Add(triangle.v1.id, indexV);
                tVertices.Add(triangle.v1.position);
                tUVS.Add(triangle.v1.uv);
            }
            indices.Add(indexV);
            if (!idindex.TryGetValue(triangle.v2.id, out indexV))
            {
                indexV = tVertices.Count;
                idindex.Add(triangle.v2.id, indexV);
                tVertices.Add(triangle.v2.position);
                tUVS.Add(triangle.v2.uv);
            }
            indices.Add(indexV);
        }

        Mesh simplifyMesh = new Mesh();
        simplifyMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        simplifyMesh.vertices = tVertices.ToArray();
        //simplifyMesh.uv = tUVS.ToArray();
        simplifyMesh.triangles = indices.ToArray();
        simplifyMesh.RecalculateNormals();

        renderVerticesNum = tVertices.Count;
        renderTrianglesNum = indices.Count / 3;
        return simplifyMesh;
    }

    public void InitMesh()
    {
        Vector3[] originVertices = originMesh.vertices;
        int[] originTriangles = originMesh.triangles;

        vertices.Clear();
        triangles.Clear();
        

        for (int i = 0; i < originVertices.Length; i++)
        {
            Vector3 vexter = originVertices[i];
            vertices.Add(new SimplifyVertex(vexter,i));
        }

        int v0;
        int v1;
        int v2;
        for (int j = 0; j < originTriangles.Length; j += 3)
        {
            v0 = originTriangles[j];
            v1 = originTriangles[j + 1];
            v2 = originTriangles[j + 2];
            if(vertices[v0] == vertices[v1] || vertices[v0] == vertices[v2] || vertices[v1] == vertices[v2])
            {
                continue;
            }
            triangles.Add(new SimplifyTriangle(vertices[v0], vertices[v1], vertices[v2]));

            vertices[v0].AppendNeighbor(vertices[v1]);
            vertices[v0].AppendNeighbor(vertices[v2]);
            vertices[v1].AppendNeighbor(vertices[v2]);
        }

        originTrianglesNum = triangles.Count;
        originVerticesNum = vertices.Count;
    }

    private SimplifyVertex MiniCostEdge()
    {
        SimplifyVertex mn = vertices[0];
        foreach(SimplifyVertex v in vertices)
        {
            if(!v.isRemoved && v.cost < mn.cost)
            {
                mn = v;
            }
        }
        return mn;
    }

    private float ComputeEdgeCollapseCost(SimplifyVertex u, SimplifyVertex v)
    {
        Vector3 tp = v.position - u.position;
        float edgelength = Vector3.SqrMagnitude(tp);
        float curvature = 0.0f; // 曲率

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
                float dotprod = Vector3.Dot(triangle.normal, sTriangle.normal);
                mincurv = Mathf.Min(mincurv, (1.0f - dotprod) / 2.0f);
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

        //if (v.isEdge) return;

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
        foreach (SimplifyVertex v in vertices)
        {
            ComputeEdgeCostAtVertex(v);
        }
    }

    // u->v 顶点v替换u 移除u
    private void Collapse(SimplifyVertex u, SimplifyVertex v)
    {
        if (v == null)
        {
            u.Remove();
            vertices.Remove(u);
            return;
        }

        int i;
        List<SimplifyVertex> tmp = new List<SimplifyVertex>();
        SimplifyTriangle simplifyTriangle;
        for(i = 0; i < u.neighbors.Count; i++)
        {
            tmp.Add(u.neighbors[i]);
        }
        // 移除包含uv边的三角形
        for(i = u.triangles.Count - 1; i >= 0; i--)
        {
            if(u.triangles[i].Contains(v))
            {
                simplifyTriangle = u.triangles[i];
                triangles.Remove(simplifyTriangle);
                simplifyTriangle.Remove();
            }
        }

        // 用v替换u 更新包含u但不包含v的三角形
        for(i = u.triangles.Count - 1; i >= 0; i--)
        {
            u.triangles[i].ReplaceSimplifyVertex(u, v);
        }

        u.Remove();
        vertices.Remove(u);

        for(i = 0; i < tmp.Count; i++)
        {
            ComputeEdgeCostAtVertex(tmp[i]);
        }
    }

}
