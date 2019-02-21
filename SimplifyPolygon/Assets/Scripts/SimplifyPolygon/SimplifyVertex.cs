using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifyVertex
{
    public Vector3 position;
    public Vector2 uv;
    public int id;

    public List<SimplifyVertex> neighbors = new List<SimplifyVertex>();
    public List<SimplifyTriangle> triangles = new List<SimplifyTriangle>();
    public float cost; // 顶点折叠代价
    public SimplifyVertex collapse; // 折叠目标顶点 
    public bool isRemoved = false; 

    public SimplifyVertex()
    {
        
    }

    public SimplifyVertex(Vector3 v, Vector2 uv, int id)
    {
        this.position = v;
        this.uv = uv;
        this.id = id;
    }

    public SimplifyVertex(Vector3 v, int id)
    {
        this.position = v;
        this.id = id;
    }

    public void AppendNeighbor(SimplifyVertex vertex)
    {
        if(!neighbors.Contains(vertex))
        {
            neighbors.Add(vertex);
        }
    }

    public void RemoveNeighbor(SimplifyVertex vertex)
    {
        if (!neighbors.Contains(vertex)) return;
        foreach(SimplifyTriangle triangle in triangles)
        {
            if (triangle.Contains(vertex)) return;
        }
       
        neighbors.Remove(vertex);
        vertex.neighbors.Remove(this);
    }

    public void Remove()
    {
        isRemoved = true;
        foreach(SimplifyVertex neighbor in neighbors)
        {
            neighbor.neighbors.Remove(this);
        }

        neighbors.Clear();
    }


}
