using UnityEngine;
using System.Collections;

public class SimplifyTriangle
{
    public SimplifyVertex v0;
    public SimplifyVertex v1;
    public SimplifyVertex v2;

    public Vector3 normal;
    public bool isRemoved = false;

    public SimplifyTriangle(SimplifyVertex v0, SimplifyVertex v1, SimplifyVertex v2)
    {
        this.v0 = v0;
        this.v1 = v1;
        this.v2 = v2;

        ComputeNormal();
    }

    public bool Contains(SimplifyVertex v)
    {
        return (v == v0 || v == v1 || v == v2);
    }

    public void ReplaceSimplifyVertex(SimplifyVertex vold, SimplifyVertex vnew)
    {
        vold.triangles.Remove(this);
        vnew.triangles.Add(this);

        if(v0 == vold)
        {
            v0 = vnew;
            v1.RemoveNeighbor(vold);
            v2.RemoveNeighbor(vold);
            v1.AppendNeighbor(vnew);
            v2.AppendNeighbor(vnew);
        }

        if(v1 == vold)
        {
            v1 = vnew;
            v0.RemoveNeighbor(vold);
            v2.RemoveNeighbor(vold);
            v0.AppendNeighbor(vnew);
            v2.AppendNeighbor(vnew);
        }

        if(v2 == vold)
        {
            v2 = vnew;
            v0.RemoveNeighbor(vold);
            v1.RemoveNeighbor(vold);
            v0.AppendNeighbor(vnew);
            v1.AppendNeighbor(vnew);
        }

        ComputeNormal();

    }

    public void Remove()
    {
        isRemoved = true;
        v0.triangles.Remove(this);
        v1.triangles.Remove(this);
        v2.triangles.Remove(this);
        v0.RemoveNeighbor(v1);
        v0.RemoveNeighbor(v2);
        v1.RemoveNeighbor(v2);
    }

    private void ComputeNormal()
    {
        Vector3 n0 = v1.position - v0.position;
        Vector3 n1 = v2.position - v1.position;
        normal = Vector3.Cross(n0, n1).normalized;
    }

   
}
