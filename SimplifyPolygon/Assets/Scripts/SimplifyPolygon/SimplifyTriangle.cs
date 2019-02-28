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
        this.v0.triangles.Add(this);
        this.v1.triangles.Add(this);
        this.v2.triangles.Add(this);
        ComputeNormal();
    }

    public bool Contains(SimplifyVertex v)
    {
        return (v == v0 || v == v1 || v == v2);
    }

    public void ReplaceSimplifyVertex(SimplifyVertex vold, SimplifyVertex vnew)
    {
        if (vold == null || vnew == null) return;
        if (vold != v0 && vold != v1 && vold != v2) return;
        if (vnew == v0 || vnew == v1 || vnew == v2) return;

        if(vold == v0)
        {
            v0 = vnew;
        }

        if(vold == v1)
        {
            v1 = vnew;
        }

        if(vold == v2)
        {
            v2 = vnew;
        }

        int i;
        vold.triangles.Remove(this);

        if (vnew.triangles.Contains(this)) return;

        vnew.triangles.Add(this);

        vold.RemoveNeighbor(v0);
        vold.RemoveNeighbor(v1);
        vold.RemoveNeighbor(v2);

        if (v0.GetSimplifyTriangleCount(this) == 1)
        {
            v0.AppendUniqueNeighbor(v1);
            v0.AppendUniqueNeighbor(v2);
        }

        if(v1.GetSimplifyTriangleCount(this) == 1)
        {
            v1.AppendUniqueNeighbor(v0);
            v1.AppendUniqueNeighbor(v2);
        }

        if(v2.GetSimplifyTriangleCount(this) == 1)
        {
            v2.AppendUniqueNeighbor(v0);
            v2.AppendUniqueNeighbor(v1);
        }

        ComputeNormal();

    }

    public void Remove()
    {
        Debug.Log("SimplifyTriangle.Remove::::");
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
        normal = Vector3.Cross(n0, n1);
        if (Vector3.SqrMagnitude(normal) == 0) return;
        normal = normal.normalized;
    }


}
